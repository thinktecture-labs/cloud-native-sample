namespace NotificationService.Configuration;

public class NotificationServiceConfiguration
{
    public NotificationServiceConfiguration()
    {
        IdentityServer = new IdentityServerConfiguration();
        Authorization = new Authorization();
    }

    public const string SectionName = "NotificationService";
    public string OnOrderProcessedMethodName { get; set; } = "onOrderProcessed";
    public string ZipkinEndpoint { get; set; }
    public IdentityServerConfiguration IdentityServer { get;set; }
    public Authorization Authorization { get; set; }
}
