namespace Gateway.Configuration;

public class GatewayConfiguration
{
    public const string SectionName = "Gateway";

    public string DaprEndpoint { get; set; } = "http://localhost:9001";
    public string ZipkinEndpoint { get; set; } = "http://localhost:9412/api/v2/spans";
    public string ConfigSection { get; set; } = "ReverseProxy";
}
