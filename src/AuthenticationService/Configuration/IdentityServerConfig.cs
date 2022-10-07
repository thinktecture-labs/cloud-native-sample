namespace AuthenticationService.Configuration;


public class IdentityServerConfig
{
    public InteractiveClientConfig InteractiveClient { get; set; }
    public AzureAdConfig AzureAd { get; set; }
    public string VirtualPath { get; set; }
    public bool AllowHttp { get; set; } = false;
}

public class AzureAdConfig
{
    public string Authority { get; set; }
    public string ClientId { get; set; }
}
public class InteractiveClientConfig
{
    public List<string> RedirectUris { get; set; }
    public string FrontChannelLogoutUri { get; set; }
    public List<string> PostLogoutRedirectUris { get; set; }
    public bool AllowOfflineAccess { get; set; }
    public List<string> AllowedCorsOrigins { get; set; }
}
