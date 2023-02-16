using System.Reflection;
using Azure.Monitor.OpenTelemetry.Exporter;
using Microsoft.IdentityModel.Tokens;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using ProductsService.Configuration;
using ProductsService.Migrations;
using ProductsService;

namespace Microsoft.AspNetCore.Builder;

public static class WebApplicationBuilderExtensions
{
    private static string serviceName = "ProductsService";
    private static string appVersion = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "0.0.0";
    private static Action<ResourceBuilder> ConfigureOpenTelemetryResource = builder => builder.AddService(serviceName,
            serviceVersion: appVersion,
            serviceInstanceId: Environment.MachineName
    );

    public static WebApplicationBuilder RunMigrations(this WebApplicationBuilder builder, ProductsServiceConfiguration cfg)
    {
        if (!String.IsNullOrWhiteSpace(cfg.ConnectionString))
        {
            try
            {
                var migrationsLogger = builder.Services
                    .BuildServiceProvider()
                    .GetRequiredService<ILogger<Migrations>>();

                var migrations = new Migrations(cfg, migrationsLogger);
                migrations.Migrate();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error while migrating database");
                Console.WriteLine(e);
                Environment.Exit(1);
            }
        }
        return builder;
    }
    public static WebApplicationBuilder ConfigureLogging(this WebApplicationBuilder builder, ProductsServiceConfiguration cfg)
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

    public static WebApplicationBuilder ConfigureTracing(this WebApplicationBuilder builder, ProductsServiceConfiguration cfg)
    {
        builder.Services
            .AddOpenTelemetry()
            .ConfigureResource(ConfigureOpenTelemetryResource)
            .WithTracing(options =>
        {
            options.AddProcessor<CustomProcessor>();
            options
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

    public static WebApplicationBuilder ConfigureMetrics(this WebApplicationBuilder builder, ProductsServiceConfiguration cfg)
    {
        builder.Services
            .AddOpenTelemetry()
            .ConfigureResource(ConfigureOpenTelemetryResource)
            .WithMetrics(options =>
        {
            options.AddRuntimeInstrumentation()
                .AddHttpClientInstrumentation()
                .AddAspNetCoreInstrumentation();
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
        });
        return builder;
    }

    public static WebApplicationBuilder ConfigureAuthN(this WebApplicationBuilder builder, ProductsServiceConfiguration cfg)
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

    public static WebApplicationBuilder ConfigureAuthZ(this WebApplicationBuilder builder, ProductsServiceConfiguration cfg)
    {
        builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy("RequiresApiScope", policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireClaim(cfg.Authorization.RequiredClaimName, cfg.Authorization.RequiredClaimValue);
            });
        });
        return builder;
    }
}
