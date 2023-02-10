using ProductsService.Data.Entities;

namespace ProductsService.Data.Repositories
{
    public interface IProductsRepository
    {
        Task<Product> CreateAsync(Product product);
        Task<IEnumerable<Product>> GetAllAsync();
        Task<Product?> GetByIdAsync(Guid id);
    }
}
