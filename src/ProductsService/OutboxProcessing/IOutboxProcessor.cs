namespace ProductsService.OutboxProcessing;

public interface IOutboxProcessor
{
    void StartProcessingIfNecessary();
}
