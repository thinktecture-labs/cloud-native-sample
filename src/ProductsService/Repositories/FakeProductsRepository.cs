using ProductsService.Entities;

namespace ProductsService.Repositories
{

    public class FakeProductsRepository : IProductsRepository
    {
        private readonly List<Product> _products = new List<Product> {
            new Product { Id = Guid.Parse("b3b749d1-fd02-4b47-8e3c-540555439db6"), Name = "Milk", Price = 0.99 }
        };

        public FakeProductsRepository(ILogger<FakeProductsRepository> logger)
        {
            Logger = logger;
        }

        public ILogger<FakeProductsRepository> Logger { get; }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            Thread.Sleep(2000);
            return await Task.Run(()=>{
                return _products;
            });
        }

        public async Task<Product?> GetByIdAsync(Guid id)
        {
            return await Task.Run(() => {
                return _products.FirstOrDefault(p=>p.Id.Equals(id));
            });
        }
    }
}
