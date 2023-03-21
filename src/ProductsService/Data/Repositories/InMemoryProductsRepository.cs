using ProductsService.Data.Entities;

namespace ProductsService.Data.Repositories;

public class InMemoryProductsRepository : IProductsRepository
{
    private readonly ILogger<InMemoryProductsRepository> _logger;
    private readonly List<Product> _products = new()
    {
        new Product(Guid.Parse("b3b749d1-fd02-4b47-8e3c-540555439db6"), "Milk", "Good milk",
                    new List<string> { "Food" }, 0.99),
        new Product(Guid.Parse("aaaaaaaa-fd02-4b47-8e3c-540555439db6"), "Coffee", "Delicious Coffee",
                    new List<string> { "Food" }, 1.99),
        new Product(Guid.Parse("08c64d77-4e3e-45f0-8455-078fca893049"), "Coke", "Tasty coke",
                    new List<string> { "Food" }, 1.49),
        new Product(Guid.Parse("f6877871-2a14-4f40-a61a-e1153592c0fb"), "Beer", "Good beer",
                    new List<string> { "Food" }, 2.99),
        new Product(Guid.Parse("9dfeb719-32e1-49a9-b55d-539f5b116dd6"), "Bread", "Delicious bread",
                    new List<string> { "Food" }, 0.99),
        new Product(Guid.Parse("1316ef5e-96b3-4976-adc4-ca97fd121078"), "Sausage", "Tasty sausage",
                    new List<string> { "Food" }, 1.49),
        new Product(Guid.Parse("d06c4115-85d5-4448-b398-464850eebf4e"), "Cheese", "Good cheese",
                    new List<string> { "Food" }, 2.99),
        new Product(Guid.Parse("4382ba39-c9e3-48bb-83b3-9f9171b4c33f"), "Chocolate", "Delicious chocolate",
                    new List<string> { "Food" }, 0.99),
        new Product(Guid.Parse("9d428166-3cb7-4513-ae0d-e1cb18ac1416"), "Candy", "Tasty candy",
                    new List<string> { "Food" }, 1.49),
        new Product(Guid.Parse("782080a1-7953-4ac0-92d8-59ec5497563b"), "Ice cream", "Good ice cream",
                    new List<string> { "Food" }, 2.99),
        new Product(Guid.Parse("128cc5a0-9a73-4cb8-896b-7d1f8e9fb5f3"), "Burger", "Delicious burger",
                    new List<string> { "Food" }, 7.99),
        new Product(Guid.Parse("a028d630-2da8-432d-ad8c-b4990d288841"), "Pizza", "Tasty pizza",
                    new List<string> { "Food" }, 9.99),
    };

    private readonly List<OutboxItem> _outbox = new ();

    public InMemoryProductsRepository(ILogger<InMemoryProductsRepository> logger)
    {
        _logger = logger;
    }

    public async Task<Product> CreateAsync(Product product)
    {
        return await Task.Run(() => {
            product.Id = Guid.NewGuid();
            _products.Add(product);
            return product;
        });
    }

    public Task<List<Product>> GetAllAsync()
    {
        _logger.LogTrace("List of all products has been requested");
        return Task.FromResult(_products);
    }

    public Task<Product?> GetByIdAsync(Guid id)
    {
        _logger.LogTrace("Product with id {Id} has been requested", id);
        return Task.FromResult(_products.FirstOrDefault(p => p.Id.Equals(id)));
    }
    
    public void AddOutboxItem(OutboxItem outboxItem) => _outbox.Add(outboxItem);

    public List<OutboxItem> GetNextOutboxItems(int pageSize = 50)
    {
        if (pageSize >= _outbox.Count)
            return _outbox.ToList();

        return _outbox.Take(pageSize).ToList();
    }

    public void RemoveOutboxItem(OutboxItem outboxItem) => _outbox.Remove(outboxItem);
}
