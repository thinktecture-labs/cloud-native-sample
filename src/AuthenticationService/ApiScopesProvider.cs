using Duende.IdentityServer.Models;
using IdentityModel;
namespace AuthenticationService;

public static class ApiScopesProvider
{

    public static IEnumerable<ApiScope> GetAll()
    {
        var sample = new ApiScope("sample");
        var admin = new ApiScope("admin");

        sample.UserClaims.Add(JwtClaimTypes.Name);

        return new ApiScope[]
        {
            sample,
            admin
        };
    }
}
