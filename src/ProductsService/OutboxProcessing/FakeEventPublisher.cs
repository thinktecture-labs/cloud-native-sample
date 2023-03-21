namespace ProductsService.OutboxProcessing;

public sealed class FakeEventPublisher : IEventPublisher
{
    public FakeEventPublisher(ILogger<FakeEventPublisher> logger)
    {
        Logger = logger;
    }

    private ILogger<FakeEventPublisher> Logger { get; }

    public Task PublishEventAsync<T>(string pubSubName,
                                     string topicName,
                                     T eventPayload,
                                     CancellationToken cancellationToken = default)
    {
        Logger.LogInformation("Published event {Event} to {PubSubName} {TopicName}",
                              eventPayload,
                              pubSubName,
                              topicName);
        return Task.CompletedTask;
    }
        
}
