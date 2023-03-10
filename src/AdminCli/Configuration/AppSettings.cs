using System;
using System.Diagnostics.CodeAnalysis;

namespace AdminCli.Configuration;

public sealed class AppSettings
{
    public IdentityServerSettings IdentityServerSettings { get; set; } = new ();
    public string GatewayUrl { get; set; } = "http://localhost:5000";
    public TokenInfo? CurrentTokenInfo { get; set; }

    public bool TryGetAccessToken([NotNullWhen(true)] out string? accessToken)
    {
        var now = DateTime.UtcNow;
        if (CurrentTokenInfo is null ||
            CurrentTokenInfo.ValidTo.AddSeconds(-30) > now)
        {
            accessToken = default;
            return false;
        }

        accessToken = CurrentTokenInfo.AccessToken;
        return true;
    }
}
