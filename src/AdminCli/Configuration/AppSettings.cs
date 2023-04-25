using System;
using System.Diagnostics.CodeAnalysis;

namespace AdminCli.Configuration;

public sealed class AppSettings
{
    public const string DefaultLocalGatewayUrl = "http://localhost:5000";
    public const string DefaultCloudGatewayUrl = "https://cn.thinktecture-demos.com/api";
    public IdentityServerSettings IdentityServerSettings { get; set; } = new ();
    public string GatewayUrl { get; set; } = DefaultLocalGatewayUrl;
    public int AccessTokenValidToThreshold { get; set; } = 10;
    public TokenInfo? CurrentTokenInfo { get; set; }

    public bool TryGetAccessToken([NotNullWhen(true)] out string? accessToken)
    {
        var now = DateTime.UtcNow;
        if (CurrentTokenInfo is null ||
            CurrentTokenInfo.ValidTo.AddSeconds(-AccessTokenValidToThreshold) < now)
        {
            accessToken = default;
            return false;
        }

        accessToken = CurrentTokenInfo.AccessToken;
        return true;
    }
}
