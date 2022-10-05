using OrdersService.Data.Entities;

namespace OrdersService.Data.Repositories;

public class FakeOrdersRepository : IOrdersRepository
{
    private readonly List<Order> _orders = new()
    {
        new Order
        {
            Id = Guid.NewGuid(),
            UserId = Guid.Empty.ToString(),
            UserName = "John Doe",
            SubmittedAt = DateTime.Now.AddDays(-1),
            Positions = new List<OrderPosition>
            {
                new()
                {
                    ProductId = Guid.Parse("b3b749d1-fd02-4b47-8e3c-540555439db6"),
                    Quantity = 4
                },
                new()
                {
                    ProductId = Guid.Parse("bbbbbbbb-fd02-4b47-8e3c-540555439db6"),
                    Quantity = 3
                },
            }
        },
        new Order
        {
            Id = Guid.NewGuid(),
            UserId = Guid.Empty.ToString(),
            UserName = "Jane Doe",
            SubmittedAt = DateTime.Now.AddDays(-2),
            Positions = new List<OrderPosition>
            {
                new()
                {
                    ProductId = Guid.Parse("aaaaaaaa-fd02-4b47-8e3c-540555439db6"),
                    Quantity = 10
                },
                new()
                {
                    ProductId = Guid.Parse("bbbbbbbb-fd02-4b47-8e3c-540555439db6"),
                    Quantity = 5
                },
                new()
                {
                    ProductId = Guid.Parse("b3b749d1-fd02-4b47-8e3c-540555439db6"),
                    Quantity = 2
                }
            }
        }
    };

    public Task AddNewOrderAsync(Order newOrder)
    {
        _orders.Add(newOrder);

        return Task.CompletedTask;
    }

    public async Task<IEnumerable<Order>> GetAllOrdersAsync()
    {
        return await Task.FromResult(_orders);
    }
}
