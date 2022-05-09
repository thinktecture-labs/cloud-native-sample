using OrdersService.Data.Entities;

namespace OrdersService.Data.Repositories;

public interface IOrdersRepository
{
    Task AddNewOrderAsync(Order newOrder);
    Task<IEnumerable<Order>> GetAllOrdersAsync();
}
