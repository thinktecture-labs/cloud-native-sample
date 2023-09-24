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
    public IdentityServerConfiguration IdentityServer { get; set; }
    public Authorization Authorization { get; set; }
}
