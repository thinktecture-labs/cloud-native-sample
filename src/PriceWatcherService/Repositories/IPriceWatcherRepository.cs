namespace PriceWatcher.Repositories;

public interface IPriceWatcherRepository
{
    bool Register(string email, Guid productId, double price);
    Task<bool> DropPriceAsync(Guid productId, double dropBy);
}
