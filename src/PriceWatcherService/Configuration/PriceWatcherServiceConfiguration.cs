using Microsoft.Extensions.Logging.Console;

namespace PriceWatcher.Configuration;

public class PriceWatcherServiceConfiguration
{
    public PriceWatcherServiceConfiguration()
    {
        IdentityServer = new IdentityServerConfiguration();
        Authorization = new Authorization();
    }
    public const string SectionName = "PriceWatcherService";
    public string PriceDropsPubSubName { get; set; } = "pricedrops";
    public string PriceDropsTopicName { get; set; } = "notifications";
    public string ConsoleFormatterName { get; set; } = ConsoleFormatterNames.Json;
    public bool DisableConsoleLog { get; set; }
    public string ZipkinEndpoint { get; set; }
    public bool ExposePrometheusMetrics {get;set;}
    public string ApplicationInsightsConnectionString {get;set;}
    public IdentityServerConfiguration IdentityServer { get;set; }
    public Authorization Authorization { get; set; }
}
