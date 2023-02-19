using System.Reflection;
using Azure.Monitor.OpenTelemetry.Exporter;
using Microsoft.IdentityModel.Tokens;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using PriceWatcher;
using PriceWatcher.Configuration;

namespace Microsoft.AspNetCore.Builder;

public static class WebApplicationBuilderExtensions
{
    private static string serviceName = "PriceWatcherService";
    private static string appVersion = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "0.0.0";
    private static Action<ResourceBuilder> ConfigureOpenTelemetryResource = builder => builder.AddService(serviceName,
            serviceVersion: appVersion,
            serviceInstanceId: Environment.MachineName
    );

    public static WebApplicationBuilder ConfigureLogging(this WebApplicationBuilder builder, PriceWatcherServiceConfiguration cfg)
    { 
        return builder;
    }

    public static WebApplicationBuilder ConfigureTracing(this WebApplicationBuilder builder, PriceWatcherServiceConfiguration cfg)
    { 
        return builder;
    }

    public static WebApplicationBuilder ConfigureMetrics(this WebApplicationBuilder builder, PriceWatcherServiceConfiguration cfg)
    {
         
        return builder;
    }

    public static WebApplicationBuilder ConfigureAuthN(this WebApplicationBuilder builder, PriceWatcherServiceConfiguration cfg)
    {
        builder.Services.AddAuthentication("Bearer")
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
