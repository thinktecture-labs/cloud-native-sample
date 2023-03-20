using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging.Console;

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
    public string ConsoleFormatterName { get; set; } = ConsoleFormatterNames.Json;
    public bool DisableConsoleLog { get; set; }
    public string ZipkinEndpoint { get; set; }
    public bool ExposePrometheusMetrics {get;set;}
    public string ApplicationInsightsConnectionString {get;set;}
    public IdentityServerConfiguration IdentityServer { get;set; }
    public Authorization Authorization { get; set; }

    public string ConnectionString { get; set; } = string.Empty;

    public bool CheckIfConnectionStringIsPresent() => !string.IsNullOrWhiteSpace(ConnectionString);

    public bool TryGetConnectionString([NotNullWhen(true)] out string? connectionString)
    {
        if (CheckIfConnectionStringIsPresent())
        {
            connectionString = ConnectionString;
            return true;
        }

        connectionString = default;
        return false;
    }
}
