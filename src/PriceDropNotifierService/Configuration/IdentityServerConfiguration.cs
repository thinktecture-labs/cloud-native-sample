namespace PriceDropNotifier.Configuration;

public class IdentityServerConfiguration
{
    public string Authority { get; set; } = string.Empty;
    public string MetadataAddress { get; set; } = string.Empty;
    public bool RequireHttpsMetadata { get; set; } = true;
}
