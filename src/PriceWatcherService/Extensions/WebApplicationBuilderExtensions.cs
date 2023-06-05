using Microsoft.IdentityModel.Tokens;
using PriceWatcher.Configuration;

namespace Microsoft.AspNetCore.Builder;

public static class WebApplicationBuilderExtensions
{
    public static WebApplicationBuilder ConfigureAuthN(this WebApplicationBuilder builder, PriceWatcherServiceConfiguration cfg)
    {
        builder
           .Services
           .AddAuthentication("Bearer")
           .AddJwtBearer("Bearer", options =>
            {
                options.Authority = cfg.IdentityServer.Authority;
                options.RequireHttpsMetadata = cfg.IdentityServer.RequireHttpsMetadata;

                if (!string.IsNullOrWhiteSpace(cfg.IdentityServer.MetadataAddress))
                {
                    options.MetadataAddress = cfg.IdentityServer.MetadataAddress;
                }
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateAudience = false,
                    ValidIssuer = cfg.IdentityServer.Authority
                };
            });
        return builder;
    }

    public static WebApplicationBuilder ConfigureAuthZ(this WebApplicationBuilder builder, PriceWatcherServiceConfiguration cfg)
    {
        builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy("RequiresApiScope", policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireClaim(cfg.Authorization.RequiredClaimName, cfg.Authorization.RequiredClaimValue);
            });

            options.AddPolicy("RequiresAdminScope", policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireClaim(cfg.Authorization.RequiredClaimName, cfg.Authorization.AdminScopeName);
            });
        });
        return builder;
    }
}
