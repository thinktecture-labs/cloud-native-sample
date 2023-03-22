using Dapr.Client;

namespace ProductsService.OutboxProcessing;

public sealed class DaprEventPublisher : IEventPublisher
{
    public DaprEventPublisher(DaprClient daprClient) => DaprClient = daprClient;

    private DaprClient DaprClient { get; }

    public Task PublishEventAsync<T>(string pubSubName,
                                     string topicName,
                                     T eventPayload,
                                     CancellationToken cancellationToken = default) =>
        DaprClient.PublishEventAsync(pubSubName,
                                     topicName,
                                     eventPayload,
                                     cancellationToken);
}
