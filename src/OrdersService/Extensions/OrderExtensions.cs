using OrdersService.Data.Entities;
using OrdersService.Models;

namespace OrdersService.Extensions;

public static class OrderExtensions
{
    public static OrderListModel ToListModel(this Order o)
    {
        return new OrderListModel
        {
            Id = o.Id,
            UserId = o.UserId,
            Positions = o.Positions == null
                ? new List<OrderPositionModel>()
                : o.Positions.Select(p => p.ToApiModel()).ToList(),
        };
    }

    public static OrderDetailsModel ToDetailsModel(this Order o)
    {
        return new OrderDetailsModel
        {
            Id = o.Id,
            UserName = o.UserName,
            UserId = o.UserId,
            Positions = o.Positions == null
                ? new List<OrderPositionModel>()
                : o.Positions.Select(p => p.ToApiModel()).ToList(),
            SubmittedAt = o.SubmittedAt
        };
    }

    public static Order ToEntity(this CreateOrderModel m, Guid id, DateTime n, string userId, string userName)
    {
        return new Order
        {
            Id = id,
            UserId = userId,
            UserName = userName,
            Positions = m.Positions == null
                ? new List<OrderPosition>()
                : m.Positions.Select(p => p.FromApiModel()).ToList(),
            SubmittedAt = n
        };
    }
}

public static class PositionExtensions
{
    public static OrderPositionModel ToApiModel(this OrderPosition op)
    {
        return new OrderPositionModel
        {
            ProductId = op.ProductId,
            Quantity = op.Quantity
        };
    }

    public static OrderPosition FromApiModel(this OrderPositionModel m)
    {
        return new OrderPosition
        {
            ProductId = m.ProductId,
            Quantity = m.Quantity
        };
    }
}
