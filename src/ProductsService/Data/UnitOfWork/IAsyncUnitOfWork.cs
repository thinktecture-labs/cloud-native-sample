namespace ProductsService.Data.UnitOfWork;

// A unit of work that also manipulates data and thus incorporates
// a SaveChangesAsync method. 
public interface IAsyncUnitOfWork : IAsyncReadOnlyUnitOfWork
{
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
