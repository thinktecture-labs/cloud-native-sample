using ProductsService.Entities;

namespace ProductsService.Repositories
{
    public interface IProductsRepository
    {
        Task<IEnumerable<Product>> GetAllAsync();
        Task<Product?> GetByIdAsync(Guid id);
    }
}
