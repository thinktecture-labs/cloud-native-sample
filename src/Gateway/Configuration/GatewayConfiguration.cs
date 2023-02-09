using Microsoft.Extensions.Logging.Console;

namespace Gateway.Configuration;

public class GatewayConfiguration
{
    public const string SectionName = "Gateway";
    public string ConsoleFormatterName { get; set; } = ConsoleFormatterNames.Json;
    public bool DisableConsoleLog { get; set; }
    public string ZipkinEndpoint { get; set; }
    public bool ExposePrometheusMetrics { get; set; }
    public string ApplicationInsightsConnectionString { get; set; }
    public string ConfigSection { get; set; } = "ReverseProxy";
    public string[] CorsOrigins { get; set; } = { "http://localhost:5005" };

    public int LimitRequestsPerMinute { get; set; } = 3000;

    public string[] DaprServiceNames { get; set; } = {
        Constants.ProductsRouteName,
        Constants.OrdersRouteName
    };
}
