using System.Text.Json;
using Dapr;
using ProductsService.Configuration;
using ProductsService.Controllers;
using ProductsService.Data.Entities;
using ProductsService.Data.UnitOfWork;

namespace ProductsService.OutboxProcessing;

// Initially, I thought that Dapr supports at-least-once semantics out of the box, but they
// only support it when delivering events to subscribers. When sending events, they do not
// persist events and try resending them if e.g. the message broker is down. See
// https://github.com/dapr/dapr/issues/4233 for details.
// In a productive environment, the outbox processor should be a separate process, especially
// when several replicas of the service are running. There should only be one
// process sending messages from the outbox at any given time.
// Currently, we can use the app settings to toggle if the Outbox Processor
// is active in this process or not using the EnableOutboxProcessing setting. If it is
// set to false, the NullOutboxProcessor is used instead (see Composition Root).
public sealed class OutboxProcessor : IHostedService, IOutboxProcessor
{
    public OutboxProcessor(IAsyncFactory<IOutboxUnitOfWork> unitOfWorkFactory,
                           IEventPublisher eventPublisher,
                           ProductsServiceConfiguration configuration,
                           ILogger<OutboxProcessor> logger)
    {
        UnitOfWorkFactory = unitOfWorkFactory;
        EventPublisher = eventPublisher;
        Configuration = configuration;
        Logger = logger;
    }

    private IAsyncFactory<IOutboxUnitOfWork> UnitOfWorkFactory { get; }
    private IEventPublisher EventPublisher { get; }
    private ProductsServiceConfiguration Configuration { get; }
    private ILogger<OutboxProcessor> Logger { get; }

    private object LockObject { get; } = new ();
    private Context? CurrentContext { get; set; } 

    public Task StartAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    public Task StopAsync(CancellationToken cancellationToken)
    {
        CurrentContext?.CancellationTokenSource.Cancel();
        return Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
    }

    public async void StartProcessingIfNecessary()
    {
        // We use a simple double-check lock to see if there already is
        // processing ongoing
        if (CurrentContext is not null)
            return;

        Context context;
        lock (LockObject)
        {
            if (CurrentContext is not null)
                return;
            
            // If we end up here, then we need to start processing
            var cancellationTokenSource = new CancellationTokenSource();
            var task = ProcessOutboxAsync(cancellationTokenSource.Token);
            context = new (task, cancellationTokenSource);
            CurrentContext = context;
        }

        // Here, we await the processing task asynchronously.
        // As this method has a return type of void (instead of Task), the continuation
        // will be scheduled on another Thread pool thread (unless the underlying unit of work
        // finishes synchronously). Thus once a caller ended up here, it can return and finish its request.
        try
        {
            await context.Task;
        }
        catch (Exception exception)
        {
            Logger.LogError(exception, "An error occurred while awaiting the outbox processing task");
        }
        finally
        {
            context.CancellationTokenSource.Dispose();
            lock (LockObject)
            {
                CurrentContext = null;
            }
        }
    }

    private async Task ProcessOutboxAsync(CancellationToken cancellationToken)
    {
        // This is the important part of the outbox processor.
        // It loads a batch of outbox items and tries to send them via the
        // event publisher. If this succeeds, the outbox item is deleted from
        // the database. This leads to a at-least-once behavior for publishing
        // messages to a broker (instead of at-most-once like in other examples in this repo).
        var random = new Random();
        while (true)
        {
            try
            {
                if (cancellationToken.IsCancellationRequested)
                    return;

                // We load not one, but up to 50 outbox items from the database to minimize
                // I/O between this service and the database. We do not incorporate any explicit
                // transaction, which is why we use a AsyncReadOnlyUnitOfWork here (when a command
                // is executed against the database, it will run within an implicit
                // transaction on MS SQL Server (I think its default isolation level is read-committed?).
                // As we try again on errors, this does not create problems.
                await using var unitOfWork = await UnitOfWorkFactory.CreateAsync(cancellationToken);
                var outboxBatch = await unitOfWork.GetNextOutboxItemsAsync(cancellationToken);
                if (outboxBatch.Count == 0)
                    return;

                foreach (var outboxItem in outboxBatch)
                {
                    if (cancellationToken.IsCancellationRequested)
                        return;

                    // Instead of switching between the outbox item types here, it would probably be better
                    // to implement some type of handlers. This would reduce bloat here and follow
                    // Bertrand Meyer's Open-Closed Principle.
                    switch (outboxItem.Type)
                    {
                        case OutboxItemTypes.PriceDrop:
                            var data = JsonSerializer.Deserialize<PriceDropMessageData>(outboxItem.Data)!;
                            var @event = new CloudEvent<PriceDropMessageData>(data)
                            {
                                Type = "com.thinktecture/price-drop-notification"
                            };
                            await EventPublisher.PublishEventAsync(Configuration.PriceDropsPubSubName,
                                                                   Configuration.PriceDropsTopicName,
                                                                   @event,
                                                                   cancellationToken);
                            Logger.LogInformation("Price drop notification {Event} was sent", @event);
                            break;
                        default:
                            Logger.LogError("Cannot process outbox item with unknown type {OutboxItem}", outboxItem);
                            break;
                    }

                    await unitOfWork.DeleteOutboxItemAsync(outboxItem, cancellationToken);
                }
            }
            catch (Exception exception)
            {
                Logger.LogError(exception, "An error occurred while processing the outbox");

                if (cancellationToken.IsCancellationRequested)
                    return;

                // If any of the third-party systems has a problem, we'll wait for a randomized time before
                // we try the next loop run.
                var waitTimeInMilliseconds = random.Next(1000, 3000);
                // ReSharper disable once MethodSupportsCancellation -- we do not want to throw here
                await Task.Delay(waitTimeInMilliseconds);
            }
        }
    }

    private sealed record Context(Task Task, CancellationTokenSource CancellationTokenSource);
}
