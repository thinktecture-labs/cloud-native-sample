using Microsoft.EntityFrameworkCore;
using OrdersService.Data.Entities;

namespace OrdersService.Data.Repositories;

public class OrdersRepository : IOrdersRepository
{
    private readonly OrdersServiceContext _dbContext;

    public OrdersRepository(OrdersServiceContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddNewOrderAsync(Order newOrder)
    {
        _dbContext.Add(newOrder);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<IEnumerable<Order>> GetAllOrdersAsync()
    {
        return await _dbContext.Orders.Include(o => o.Positions).ToListAsync();
    }
}
