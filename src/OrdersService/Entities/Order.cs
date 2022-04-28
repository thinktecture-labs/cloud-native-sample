using Microsoft.Extensions.Options;
using OrdersService.Models;

namespace OrdersService.Entities;

public class Order
{
    public Guid Id { get; set; }
    public string UserId { get; set; }
    public string? UserName {get;set;}
    public IEnumerable<OrderPosition>? Positions {get;set;}
    public DateTime SubmittedAt {get;set;}
}

public class OrderPosition
{
    public Guid ProductId {get;set;}
    public int Quantity {get;set;}

}
