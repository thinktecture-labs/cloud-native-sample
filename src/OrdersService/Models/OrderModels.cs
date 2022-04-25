using Swashbuckle.AspNetCore.Annotations;

namespace OrdersService.Models
{
    [SwaggerSchema()]
    public class CreateOrderModel
    {
        [SwaggerSchema("Name of the customer")]
        public string? CustomerName {get;set;}
        [SwaggerSchema("List of order positions")]
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
