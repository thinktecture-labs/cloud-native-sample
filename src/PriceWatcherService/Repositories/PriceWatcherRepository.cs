using Dapr.Client;
using PriceWatcher.Entities;

namespace PriceWatcher.Repositories;

public class PriceWatcherRepository : IPriceWatcherRepository
{
    private readonly DaprClient _client;
    private readonly ILogger<PriceWatcherRepository> _logger;

    private readonly List<Product> _products = new List<Product>()
    {
        new Product(Guid.Parse("b3b749d1-fd02-4b47-8e3c-540555439db6"), "Milk",0.99),
        new Product(Guid.Parse("aaaaaaaa-fd02-4b47-8e3c-540555439db6"), "Coffee",1.99),
        new Product(Guid.Parse("bbbbbbbb-fd02-4b47-8e3c-540555439db6"), "Coke", 1.49),
    };
    private readonly List<Watcher> _watchers = new List<Watcher>();

    public PriceWatcherRepository(DaprClient client, ILogger<PriceWatcherRepository> logger)
    {
        _client = client;
        _logger = logger;
    }
    
    public bool Register(string email, Guid productId, double price)
    {
        if (!_products.Any(p => p.Id.Equals(productId)))
        {
            _logger.LogInformation("Product {ProductId} not found", productId);
            return false;
        }
        
        var found = _watchers.FirstOrDefault(w => w.Email == email && w.ProductId == productId);
        if (found != null)
        {
            found.Price = price;
            return true;
        }

        _watchers.Add(new Watcher
        {
            Email = email,
            ProductId = productId,
            Price = price
        });
        _logger.LogInformation("Price Watch for Product {ProductId} registered  ({Watcher})", productId, email);
        return true;
    }

    public bool DropPrice(Guid productId, double dropBy)
    {
        var found = _products.FirstOrDefault(p => p.Id.Equals(productId));
        if (found == null)
        {
            _logger.LogInformation("Product {ProductId} not found", productId);
            return false;
        }

        found.Price *= (1 - dropBy);
        _logger.LogInformation("Price for {ProductName} dropped by {DroppedBy}% - new price: {NewPrice} ", found.Name, dropBy*100, found.Price);
        _watchers
            .Where(w=>w.ProductId.Equals(productId))
            .Where(w=>w.Price > found.Price)
            .ToList()
            .ForEach(w =>
            {
                _logger.LogInformation("Issue notification for {Watcher} because price dropped for {ProductName} ({ProductId})", w.Email, found.Name, found.Id);
                _client.PublishEventAsync("pricedrops", "notifications", new {
                    Recipient = w.Email,
                    ProductName = found.Name,
                    Price = found.Price
                });
                
            });
        return true;
    }
}
