using ProductsService.Entities;
using ProductsService.Models;

namespace ProductsService.Repositories
{
    public class FakeProductsRepository : IProductsRepository
    {
        private readonly ILogger<FakeProductsRepository> _logger;
        private readonly List<Product> _products = new()
        {
            new Product(Guid.Parse("b3b749d1-fd02-4b47-8e3c-540555439db6"), "Milk", "Good milk",
                new List<string> { "Food" }, 0.99),
            new Product(Guid.Parse("aaaaaaaa-fd02-4b47-8e3c-540555439db6"), "Coffee", "Delicious Coffee",
                new List<string> { "Food" }, 1.99),
            new Product(Guid.Parse("bbbbbbbb-fd02-4b47-8e3c-540555439db6"), "Coke", "Tasty coke",
                new List<string> { "Food" }, 1.49),
        };

        public FakeProductsRepository(ILogger<FakeProductsRepository> logger)
        {
            _logger = logger;
        }

        public async Task<IEnumerable<ProductListModel>> GetAllAsync()
        {
            _logger.LogTrace("List of all products has been requested");
            return await Task.Run(() => { return _products.Select(p => p.ToListModel()); });
        }

        public async Task<ProductDetailsModel?> GetByIdAsync(Guid id)
        {
            _logger.LogTrace("Product with id {Id} has been requested", id);

            return await Task.Run(() =>
            {
                var found = _products.FirstOrDefault(p => p.Id.Equals(id));

                return found?.ToDetailsModel();
            });
        }
    }
}
