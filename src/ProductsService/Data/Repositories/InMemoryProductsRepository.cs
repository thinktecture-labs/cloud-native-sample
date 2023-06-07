using ProductsService.Data.Entities;

namespace ProductsService.Data.Repositories
{
    public class InMemoryProductsRepository : IProductsRepository
    {
        private readonly ILogger<InMemoryProductsRepository> _logger;
        private readonly List<Product> _products = InMemoryProducts.Products;

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

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            _logger.LogTrace("List of all products has been requested");

            return await Task.Run(() => { return _products; });
        }

        public async Task<Product?> GetByIdAsync(Guid id)
        {
            _logger.LogTrace("Product with id {Id} has been requested", id);

            return await Task.Run(() =>
            {
                var found = _products.FirstOrDefault(p => p.Id.Equals(id));

                return found;
            });
        }
    }
}
