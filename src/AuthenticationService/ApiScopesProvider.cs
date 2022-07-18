using Duende.IdentityServer.Models;

namespace AuthenticationService;

public static class ApiScopesProvider
{

    public static IEnumerable<ApiScope> GetAll =>
        new ApiScope[]
        {
            new ApiScope("sample")
        };
}
