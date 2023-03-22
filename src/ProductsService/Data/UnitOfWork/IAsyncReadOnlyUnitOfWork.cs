namespace ProductsService.Data.UnitOfWork;

// A unit of work that only reads data and thus doesn't need the SaveChangesAsync method
public interface IAsyncReadOnlyUnitOfWork : IAsyncDisposable, IDisposable { }
