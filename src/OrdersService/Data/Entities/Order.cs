namespace OrdersService.Data.Entities;

public class Order
{
    public Guid Id { get; set; }
    public string UserId { get; set; }
    public string? UserName { get; set; }
    public List<OrderPosition>? Positions { get; set; }
    public DateTime SubmittedAt { get; set; }
}

public class OrderPosition
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
}
