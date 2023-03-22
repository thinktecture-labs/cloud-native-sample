using ProductsService.Data.Entities;
using ProductsService.Data.Repositories;
using ProductsService.Data.UnitOfWork.InMemory;

namespace ProductsService.OutboxProcessing;

public sealed class InMemoryOutboxUnitOfWork : InMemoryAsyncReadOnlyUnitOfWork, IOutboxUnitOfWork
{
    public InMemoryOutboxUnitOfWork(InMemoryProductsRepository repository)
    {
        Repository = repository;
    }

    private InMemoryProductsRepository Repository { get; }

    public Task<List<OutboxItem>> GetNextOutboxItemsAsync(CancellationToken cancellationToken) =>
        Task.FromResult(Repository.GetNextOutboxItems());

    public Task DeleteOutboxItemAsync(OutboxItem outboxItem, CancellationToken cancellationToken)
    {
        Repository.RemoveOutboxItem(outboxItem);
        return Task.CompletedTask;
    }
}
