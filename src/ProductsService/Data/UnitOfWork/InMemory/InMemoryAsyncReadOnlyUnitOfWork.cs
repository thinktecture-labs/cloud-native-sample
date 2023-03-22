namespace ProductsService.Data.UnitOfWork.InMemory;

public abstract class InMemoryAsyncReadOnlyUnitOfWork : IAsyncReadOnlyUnitOfWork, IInitializeAsync
{
    public ValueTask DisposeAsync() => default;
    public void Dispose() { }
    bool IInitializeAsync.IsInitialized => true;
    public Task InitializeAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
}
