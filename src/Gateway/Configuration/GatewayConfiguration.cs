namespace Gateway.Configuration;

public class GatewayConfiguration
{
    public const string SectionName = "Gateway";
    public string ZipkinEndpoint { get; set; } = "http://localhost:9412/api/v2/spans";
    public string ConfigSection { get; set; } = "ReverseProxy";
    public string[] CorsOrigins { get; set; } = { "http://localhost:5005" };
    public string[] DaprServiceNames { get; set; } = { Constants.ProductsRouteName, Constants.OrdersRouteName };

}
