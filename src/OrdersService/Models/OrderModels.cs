namespace OrdersService.Models
{
    public class CreateOrderModel
    {
        public string? CustomerName {get;set;}
        public IEnumerable<OrderPositionModel>? Positions { get; set; }
    }

    public readonly record struct OrderDetailsModel(Guid Id, string? CustomerName,
        IEnumerable<OrderPositionModel> Positions, DateTime SubmittedAt);
    
    
    public class OrderPositionModel
    {
        public Guid ProductId {get;set;}
        public string? ProductName {get;set;}
        public int Quantity {get;set;}
    }
}
