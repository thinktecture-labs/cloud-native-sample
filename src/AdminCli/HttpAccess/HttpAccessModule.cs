using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.DependencyInjection;

namespace AdminCli.HttpAccess;

public static class HttpAccessModule
{
    public static IServiceCollection AddHttpAccess(this IServiceCollection services) =>
        services.AddHttpClient()
                .AddSingleton(new JwtSecurityTokenHandler())
                .AddSingleton<HttpIdentityService>()
                .AddSingleton<IHttpService, HttpService>();
}
