using ProductsService.Entities;
using ProductsService.Models;

namespace ProductsService.Repositories
{
    public interface IProductsRepository
    {
        Task<IEnumerable<ProductListModel>> GetAllAsync();
        Task<ProductDetailsModel?> GetByIdAsync(Guid id);
    }
}
