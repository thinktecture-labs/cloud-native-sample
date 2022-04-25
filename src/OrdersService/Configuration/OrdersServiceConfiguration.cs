namespace OrdersService.Configuration;

public class OrdersServiceConfiguration
{
    public const string SectionName = "OrdersService";
    public string CreateOrderPubSubName { get; set; } = "orders";
    public string CreateOrderTopicName { get; set; } = "new-orders";

}
