namespace AdminCli.Configuration;

public sealed class IdentityServerSettings
{
    public const string DefaultLocalServerUrl = "http://localhost:5009";
    public const string DefaultCloudServerUrl = "https://cn.thinktecture-demos.com/login";
        
    // We shouldn't normally expose sensitive data here,
    // but for the sake of simplicity, we just keep the
    // credentials in here.
    public string ServerUrl { get; set; } = DefaultLocalServerUrl;
    public string ClientId { get; set; } = "admin-cli";
    public string ClientSecret { get; set; } = "46E345BC-9C72-4694-8BFF-27AA6BB2B6A2";
    public string Scopes { get; set; } = "admin sample";
}
