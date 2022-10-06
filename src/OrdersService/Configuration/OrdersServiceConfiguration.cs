namespace OrdersService.Configuration;

public class OrdersServiceConfiguration
{
    public OrdersServiceConfiguration()
    {
        IdentityServer = new IdentityServerConfiguration();
        Authorization = new Authorization();
    }
    public const string SectionName = "OrdersService";
    public string CreateOrderPubSubName { get; set; } = "orders";
    public string CreateOrderTopicName { get; set; } = "new_orders";
    public string ZipkinEndpoint { get; set; }
    public IdentityServerConfiguration IdentityServer { get;set; }
    public Authorization Authorization { get; set; }
}

public class Authorization
{
    public string RequiredClaimName { get; set; } = "scope";
    public string RequiredClaimValue {get;set;} = "sample";
}

public class IdentityServerConfiguration 
{
    public string Authority {get;set;}
    public string MetadataAddress {get;set;}
    public bool RequireHttpsMetadata { get; set;} = true;

}
