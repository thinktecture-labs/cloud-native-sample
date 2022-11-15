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
    public string OnOrderProcessedMethodName { get; set; } = "onOrderProcessed";
    public TraceSystem TraceSystem { get; set; } = TraceSystem.Zipkin;
    public string TraceEndpoint { get; set; }
    public IdentityServerConfiguration IdentityServer { get;set; }
    public Authorization Authorization { get; set; }
}
