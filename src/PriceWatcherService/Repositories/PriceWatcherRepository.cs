using Dapr;
using Dapr.Client;
using PriceWatcher.Configuration;
using PriceWatcher.Entities;
using PriceWatcher.Models;

namespace PriceWatcher.Repositories;

public class PriceWatcherRepository : IPriceWatcherRepository
{
    private readonly DaprClient _dapr;
    private readonly PriceWatcherServiceConfiguration _cfg;
    private readonly ILogger<PriceWatcherRepository> _logger;
    private readonly List<Watcher> _watchers = new List<Watcher>();

    private static readonly List<Product> _products = new List<Product> {
        new Product { Id = Guid.Parse("08ae4294-47e1-4b76-8cd3-052d6308d699"), Name = "Ice cream", Description = "Cool down on hot days", Price = 4.49 },
        new Product { Id = Guid.Parse("67611138-7dc1-42d9-b910-42c5d0247c52"), Name = "Bread", Description = "Yummy! Fresh bread smells super good", Price = 4.29 },
        new Product { Id = Guid.Parse("fed436f8-76a2-4ce0-83a2-6bdb0fed705b"), Name = "Coffee", Description = "Delicious Coffee", Price = 2.49 },
        new Product { Id = Guid.Parse("870d8ca1-1936-41a2-9f40-7d399f29ac38"), Name = "Bacon Burger", Description = "Everything is better with bacon", Price = 8.99 },
        new Product { Id = Guid.Parse("d96798d2-b429-4842-9a29-9a2a448d4ff2"), Name = "Whisky", Description = "Gentle drink for cold evenings", Price = 49.99 },
        new Product { Id = Guid.Parse("83fc59d6-9e20-450a-84c0-c8bc8fd80ee1"), Name = "Coke", Description = "Tasty coke", Price = 1.99 },
        new Product { Id = Guid.Parse("e2810857-327d-47d1-918c-cf3e3709d2d8"), Name = "Sausage", Description = "Time for some BBQ", Price = 3.79 },
        new Product { Id = Guid.Parse("525f1786-c045-46b1-aac2-d06da196bac4"), Name = "Beer", Description = "Tasty craft beer", Price = 3.99 },
        new Product { Id = Guid.Parse("9b699928-4600-44bf-9923-ec41a428b809"), Name = "Coffee", Description = "Delicious", Price = 2.49 },
        new Product { Id = Guid.Parse("2620540e-bfcb-4a06-87a4-f6ed2b3c069b"), Name = "Pizza", Description = "It comes with Bacon. You know! Because everything is better with bacon", Price = 7.99 }
    };

    public PriceWatcherRepository(DaprClient dapr, PriceWatcherServiceConfiguration cfg, ILogger<PriceWatcherRepository> logger)
    {
        _dapr = dapr;
        _cfg = cfg;
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
        _logger.LogInformation("Price for {ProductName} dropped by {DroppedBy}% - new price: {NewPrice} ", found.Name, dropBy * 100, found.Price);
        _watchers
            .Where(w => w.ProductId.Equals(productId))
            .Where(w => w.Price > found.Price)
            .ToList()
            .ForEach(async w =>
            {
                _logger.LogInformation("Issue notification for {Watcher} because price dropped for {ProductName} ({ProductId})", w.Email, found.Name, found.Id);
                _logger.LogWarning($"Publishing message in {_cfg.PriceDropsPubSubName}:{_cfg.PriceDropsTopicName}");
                var model = new PriceDropNotificationModel
                {
                    Recipient = w.Email,
                    ProductName = found.Name,
                    Price = found.Price
                };
                var cloudEvent = new CloudEvent<PriceDropNotificationModel>(model){
                    Type = "com.thinktecture/price-drop-notification"
                }; 

                await _dapr.PublishEventAsync<CloudEvent<PriceDropNotificationModel>>(
                    _cfg.PriceDropsPubSubName, 
                    _cfg.PriceDropsTopicName,
                    cloudEvent,
                    cancellationToken: CancellationToken.None);

            });
        return true;
    }
}
