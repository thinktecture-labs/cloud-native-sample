using Microsoft.Extensions.Logging.Console;

namespace PriceDropNotifier.Configuration;

public class PriceDropNotifierServiceConfiguration
{
    public PriceDropNotifierServiceConfiguration()
    {
        IdentityServer = new ();
        Authorization = new ();
    }
    
    public const string SectionName = "PriceDropNotifierService";

    public string ConsoleFormatterName { get; set; } = ConsoleFormatterNames.Json;
    public bool DisableConsoleLog { get; set; }
    public string ZipkinEndpoint { get; set; } = string.Empty;
    public bool ExposePrometheusMetrics { get; set; }
    public string ApplicationInsightsConnectionString { get; set; } = string.Empty;
    public IdentityServerConfiguration IdentityServer { get; set; }
    public Authorization Authorization { get; set; }
    public string NotificationBindingName { get; set; } = "email";
    public string NotificationBindingOperation { get; set; } = "create";
}
