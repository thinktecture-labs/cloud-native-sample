namespace ProductsService.OutboxProcessing;

public interface IEventPublisher
{
    Task PublishEventAsync<T>(string pubSubName,
                              string topicName,
                              T eventPayload,
                              CancellationToken cancellationToken = default);
}
