namespace ProductsService.Data.UnitOfWork.InMemory;

public abstract class InMemoryAsyncUnitOfWork : InMemoryAsyncReadOnlyUnitOfWork, IAsyncUnitOfWork
{
    public Task SaveChangesAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
}
