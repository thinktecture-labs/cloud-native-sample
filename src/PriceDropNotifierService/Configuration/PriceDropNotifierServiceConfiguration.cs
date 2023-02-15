using Microsoft.Extensions.Logging.Console;

namespace PriceDropNotifier.Configuration;

public class PriceDropNotifierServiceConfiguration
{
    public PriceDropNotifierServiceConfiguration()
    {
        IdentityServer = new IdentityServerConfiguration();
        Authorization = new Authorization();
    }
    public const string SectionName = "PriceDropNotifierService";
    public string NotificationBindingName = "email";
    public string NotificationBindingOperation = "create";
    public string ConsoleFormatterName { get; set; } = ConsoleFormatterNames.Json;
    public bool DisableConsoleLog { get; set; }
    public string ZipkinEndpoint { get; set; }
    public bool ExposePrometheusMetrics {get;set;}
    public string ApplicationInsightsConnectionString {get;set;}
    public IdentityServerConfiguration IdentityServer { get;set; }
    public Authorization Authorization { get; set; }
}
