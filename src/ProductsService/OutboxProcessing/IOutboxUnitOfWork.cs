using ProductsService.Data.Entities;
using ProductsService.Data.UnitOfWork;

namespace ProductsService.OutboxProcessing;

public interface IOutboxUnitOfWork : IAsyncReadOnlyUnitOfWork
{
    Task<List<OutboxItem>> GetNextOutboxItemsAsync(CancellationToken cancellationToken = default);

    Task DeleteOutboxItemAsync(OutboxItem outboxItem, CancellationToken cancellationToken = default);
}
