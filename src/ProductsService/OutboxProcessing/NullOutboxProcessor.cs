namespace ProductsService.OutboxProcessing;

public sealed class NullOutboxProcessor : IOutboxProcessor
{
    public void StartProcessingIfNecessary() { }
}
