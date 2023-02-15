using System.Reflection;
using Azure.Monitor.OpenTelemetry.Exporter;
using Microsoft.IdentityModel.Tokens;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using NotificationService.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using NotificationService;

namespace Microsoft.AspNetCore.Builder;

public static class WebApplicationBuilderExtensions
{
    private static string serviceName = "NotificationsService";
    private static string appVersion = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "0.0.0";
   
    private static Action<ResourceBuilder> ConfigureOpenTelemetryResource = builder => builder.AddService(serviceName,
            serviceVersion: appVersion,
            serviceInstanceId: Environment.MachineName
    );

    public static WebApplicationBuilder ConfigureLogging(this WebApplicationBuilder builder, NotificationServiceConfiguration cfg)
    {
        builder.Logging.ClearProviders();
        if (System.Diagnostics.Debugger.IsAttached)
        {
            builder.Logging.AddDebug();
        }
        if (!cfg.DisableConsoleLog)
        {
            builder.Logging.AddConsole(options =>
            {
                options.FormatterName = cfg.ConsoleFormatterName;
            });
        }

        builder.Logging.AddOpenTelemetry(options =>
        {
            var b = ResourceBuilder.CreateDefault();
            ConfigureOpenTelemetryResource(b);
            options.SetResourceBuilder(b);

            if (!string.IsNullOrWhiteSpace(cfg.ApplicationInsightsConnectionString))
            {
                Console.WriteLine("Logs: Sending to Azure Monitor");
                options.AddAzureMonitorLogExporter(o => o.ConnectionString = cfg.ApplicationInsightsConnectionString);
            }

        });
        return builder;
    }

    public static WebApplicationBuilder ConfigureTracing(this WebApplicationBuilder builder, NotificationServiceConfiguration cfg)
    {
        builder.Services.AddOpenTelemetry()
            .ConfigureResource(ConfigureOpenTelemetryResource)
            .WithTracing(options =>
            {
                options
                    .AddProcessor<CustomProcessor>()
                    .AddHttpClientInstrumentation()
                    .AddAspNetCoreInstrumentation();

                if (!string.IsNullOrWhiteSpace(cfg.ApplicationInsightsConnectionString))
                {
                    Console.WriteLine("Tracing: Adding Azure Monitor Exporter");
                    options.AddAzureMonitorTraceExporter(o => o.ConnectionString = cfg.ApplicationInsightsConnectionString);
                }
                if (!string.IsNullOrWhiteSpace(cfg.ZipkinEndpoint))
                {
                    Console.WriteLine("Tracing: Adding Zipkin Exporter (" + cfg.ZipkinEndpoint + ")");
                    options.AddZipkinExporter(o =>
                    {
                        o.Endpoint = new Uri(cfg.ZipkinEndpoint);
                    });
                }
            }).StartWithHost();
        return builder;
    }

    public static WebApplicationBuilder ConfigureMetrics(this WebApplicationBuilder builder, NotificationServiceConfiguration cfg)
    {
        builder.Services.AddOpenTelemetry()
            .ConfigureResource(ConfigureOpenTelemetryResource)
            .WithMetrics(options =>
        {
            options
                .AddRuntimeInstrumentation()
                .AddHttpClientInstrumentation()
                .AddAspNetCoreInstrumentation()
                .AddMeter(CustomMetrics.Default.Name);

            if (!string.IsNullOrWhiteSpace(cfg.ApplicationInsightsConnectionString))
            {
                Console.WriteLine("Metrics: Adding Azure Monitor Exporter");
                options.AddAzureMonitorMetricExporter(o => o.ConnectionString = cfg.ApplicationInsightsConnectionString);
            }
            if (cfg.ExposePrometheusMetrics)
            {
                Console.WriteLine("Exposing Prometheus Metrics");
                options.AddPrometheusExporter();
            }
        }).StartWithHost();
        return builder;
    }

    public static WebApplicationBuilder ConfigureAuthN(this WebApplicationBuilder builder, NotificationServiceConfiguration cfg)
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
                    ValidIssuer = cfg.IdentityServer.Authority,
                };
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];

                        // If the request is for our hub...
                        var path = context.HttpContext.Request.Path;
                        if (!string.IsNullOrEmpty(accessToken) &&
                            (path.StartsWithSegments(Constants.NotificationHubEndpoint)))
                        {
                            // Read the token out of the query string
                            context.Token = accessToken;
                        }
                        return Task.CompletedTask;
                    }
                };
            });
        Console.WriteLine($"Authentication configured for {cfg.IdentityServer.Authority} (require HTTPs: {cfg.IdentityServer.RequireHttpsMetadata}) Metadata Address {cfg.IdentityServer.MetadataAddress}");
        return builder;
    }

    public static WebApplicationBuilder ConfigureAuthZ(this WebApplicationBuilder builder, NotificationServiceConfiguration cfg)
    {
        builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy(Constants.AuthorizationPolicyName, policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireClaim(cfg.Authorization.RequiredClaimName, cfg.Authorization.RequiredClaimValue);
            });
        });
        Console.WriteLine($"Authorization configured with Policy {Constants.AuthorizationPolicyName} RequiredClaim: {cfg.Authorization.RequiredClaimName} {cfg.Authorization.RequiredClaimValue}");
        return builder;
    }
}
