using OrdersService.Entities;

namespace OrdersService.Repositories;

public interface IOrdersRepository
{
    Task<IEnumerable<Order>> GetAllOrdersAsync();
}
