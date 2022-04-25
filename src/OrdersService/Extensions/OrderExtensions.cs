// ReSharper disable once CheckNamespace

using OrdersService.Entities;
using OrdersService.Models;

namespace OrdersService;

public static class OrderExtensions
{
    public static OrderDetailsModel ToDetailsModel(this Order o)
    {
        return new OrderDetailsModel(o.Id, o.CustomerName, o.Positions == null? Enumerable.Empty<OrderPositionModel>() : o.Positions.Select(p=> p.ToApiModel()), o.SubmittedAt);
    }

    public static Order ToEntity(this CreateOrderModel m, Guid id, DateTime n)
    {
        return new Order
        {
            Id = id,
            UserId = m.UserId,
            CustomerName = m.CustomerName,
            Positions = m.Positions == null? Enumerable.Empty<OrderPosition>() : m.Positions.Select(p=> p.FromApiModel()),
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
            Quantity = op.Quantity,
            ProductName = op.ProductName
        };
    }
    
    public static OrderPosition FromApiModel(this OrderPositionModel m)
    {
        return new OrderPosition
        {
            ProductId = m.ProductId,
            ProductName = m.ProductName,
            Quantity = m.Quantity
        };
    }
}
