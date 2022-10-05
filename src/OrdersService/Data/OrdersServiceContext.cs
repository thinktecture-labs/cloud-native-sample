using Microsoft.EntityFrameworkCore;
using OrdersService.Data.Entities;

namespace OrdersService.Data;

public class OrdersServiceContext : DbContext
{
    public OrdersServiceContext(DbContextOptions<OrdersServiceContext> options)
        : base(options)
    {
    }

    public DbSet<Order> Orders { get; set; }
}
