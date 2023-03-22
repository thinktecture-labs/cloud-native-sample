namespace ProductsService.Data.UnitOfWork;

public interface IInitializeAsync
{
    bool IsInitialized { get; }
    Task InitializeAsync(CancellationToken cancellationToken = default);
}
