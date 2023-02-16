using AuthenticationService.Configuration;
using Duende.IdentityServer.Models;

namespace AuthenticationService;

public static class ClientsProvider
{

    public static IEnumerable<Client> GetAll(InteractiveClientConfig interactiveClientConfig) =>
        new Client[]
        {
            new Client
            {

                ClientId = "cc",
                ClientName = "Client Credentials Client",

                AllowedGrantTypes = GrantTypes.ClientCredentials,
                ClientSecrets = { new Secret("612dc56a-abff-4885-92ad-1bb7c266e324".Sha256()) },

                AllowedScopes = { "sample" }
            },

            new Client
            {
                ClientId = "interactive",
                ClientSecrets = { new Secret("49C1A7E1-0C79-4A89-A3D6-A37998FB86B0".Sha256()) },

                RequireClientSecret = false,

                AllowedGrantTypes = GrantTypes.Code,

                RedirectUris = interactiveClientConfig.RedirectUris,
                FrontChannelLogoutUri = interactiveClientConfig.FrontChannelLogoutUri,
                PostLogoutRedirectUris = interactiveClientConfig.PostLogoutRedirectUris,

                AllowOfflineAccess = interactiveClientConfig.AllowOfflineAccess,
                AllowedScopes = { "openid", "profile", "sample" },
                AllowedCorsOrigins = interactiveClientConfig.AllowedCorsOrigins,
            },
            new Client {
                ClientId = "postman",
                ClientSecrets = { new Secret("e3e4d08b-430f-43fa-bffc-9eaf85f55c4d".Sha256()) },

                RequireClientSecret = false,

                AllowedGrantTypes = GrantTypes.Code,

                RedirectUris = interactiveClientConfig.RedirectUris,
                FrontChannelLogoutUri = interactiveClientConfig.FrontChannelLogoutUri,
                PostLogoutRedirectUris = interactiveClientConfig.PostLogoutRedirectUris,

                AllowOfflineAccess = interactiveClientConfig.AllowOfflineAccess,
                AllowedScopes = { "openid", "profile", "sample", "admin" },
                AllowedCorsOrigins = interactiveClientConfig.AllowedCorsOrigins,
            },
        };
}
