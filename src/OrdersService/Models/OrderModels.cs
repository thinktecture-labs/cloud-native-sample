using Swashbuckle.AspNetCore.Annotations;

namespace OrdersService.Models
{
    [SwaggerSchema()]
    public class CreateOrderModel
    {
        [SwaggerSchema("User's unique identifier")]
        public string? UserId { get; set; }

        [SwaggerSchema("List of order positions")]
        public List<OrderPositionModel> Positions { get; set; }
    }

    public class OrderListModel
    {
        public Guid Id { get; init; }
        public string UserId { get; set; }
        public List<OrderPositionModel> Positions { get; init; }
    }

    public class OrderDetailsModel
    {
        public Guid Id { get; init; }
        public string UserId { get; set; }
        public string? UserName { get; init; }
        public List<OrderPositionModel> Positions { get; init; }
        public DateTime SubmittedAt { get; init; }
    }

    public class OrderPositionModel
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
