using OrdersService.Entities;

namespace OrdersService.Repositories;

public interface IOrdersRepository
{
    Task AddNewOrderAsync();
    Task<IEnumerable<Order>> GetAllOrdersAsync();
}
