namespace PriceWatcher.Repositories;

public interface IPriceWatcherRepository
{
    bool Register(string email, Guid productId, double price);
    bool  DropPrice(Guid productId, double dropBy);
}
