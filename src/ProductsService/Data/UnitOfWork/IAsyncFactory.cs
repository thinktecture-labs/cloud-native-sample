namespace ProductsService.Data.UnitOfWork;

public interface IAsyncFactory<T>
{
    ValueTask<T> CreateAsync(CancellationToken cancellationToken = default);
}
