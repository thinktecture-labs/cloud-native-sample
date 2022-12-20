using Microsoft.Extensions.Logging.Console;

namespace NotificationService.Configuration;

public class NotificationServiceConfiguration
{
    public NotificationServiceConfiguration()
    {
        IdentityServer = new IdentityServerConfiguration();
        Authorization = new Authorization();
    }
    public const string SectionName = "NotificationService";
    public string ConsoleFormatterName { get; set; } = ConsoleFormatterNames.Json;
    public bool DisableConsoleLog { get; set; }
    public string OnOrderProcessedMethodName { get; set; } = "onOrderProcessed";
    public string ZipkinEndpoint { get; set; }
    public bool ExposePrometheusMetrics {get;set;}
    public string ApplicationInsightsConnectionString {get;set;}
    public IdentityServerConfiguration IdentityServer { get;set; }
    public Authorization Authorization { get; set; }
}
