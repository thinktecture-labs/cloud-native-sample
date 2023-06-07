using Dapr;
using Dapr.Client;
using PriceWatcher.Configuration;
using PriceWatcher.Entities;
using PriceWatcher.Models;
using ProductsService.Data;
using ProductsService.Data.Entities;

namespace PriceWatcher.Repositories;

public class PriceWatcherRepository : IPriceWatcherRepository
{
    private readonly DaprClient _dapr;
    private readonly PriceWatcherServiceConfiguration _cfg;
    private readonly ILogger<PriceWatcherRepository> _logger;
    private readonly List<Watcher> _watchers = new List<Watcher>();
    
    public PriceWatcherRepository(DaprClient dapr, PriceWatcherServiceConfiguration cfg, ILogger<PriceWatcherRepository> logger)
    {
        _dapr = dapr;
        _cfg = cfg;
        _logger = logger;
    }

    private static List<Product> Products => InMemoryProducts.Products;

    public bool Register(string email, Guid productId, double price)
    {
        if (!Products.Any(p => p.Id.Equals(productId)))
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

        var found = Products.FirstOrDefault(p => p.Id.Equals(productId));
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
