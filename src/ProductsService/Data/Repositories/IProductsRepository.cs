using ProductsService.Data.Entities;

namespace ProductsService.Data.Repositories
{
    public interface IProductsRepository
    {
        Task<IEnumerable<Product>> GetAllAsync();
        Task<Product?> GetByIdAsync(Guid id);
    }
}
