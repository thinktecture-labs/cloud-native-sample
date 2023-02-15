using System.Reflection;
using AuthenticationService;
using AuthenticationService.Configuration;
using Azure.Monitor.OpenTelemetry.Exporter;
using Duende.IdentityServer;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
namespace Microsoft.AspNetCore.Builder;

public static class WebApplicationBuilderExtensions
{
    private static string serviceName = "AuthenticationService";
    private static string appVersion = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "0.0.0";
    private static Action<ResourceBuilder> ConfigureOpenTelemetryResource = builder => builder.AddService(serviceName,
            serviceVersion: appVersion,
            serviceInstanceId: Environment.MachineName
    );

    public static WebApplicationBuilder ConfigureLogging(this WebApplicationBuilder builder, IdentityServerConfig cfg)
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

    public static WebApplicationBuilder ConfigureTracing(this WebApplicationBuilder builder, IdentityServerConfig cfg)
    {
        builder.Services
            .AddOpenTelemetry()
            .ConfigureResource(ConfigureOpenTelemetryResource)
            .WithTracing(options =>
            {
                options.AddProcessor<CustomProcessor>();
                
                options.AddSource(IdentityServerConstants.Tracing.Basic)
                    .AddSource(IdentityServerConstants.Tracing.Cache)
                    .AddSource(IdentityServerConstants.Tracing.Services)
                    .AddSource(IdentityServerConstants.Tracing.Stores)
                    .AddSource(IdentityServerConstants.Tracing.Validation);
                
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

    public static WebApplicationBuilder ConfigureMetrics(this WebApplicationBuilder builder, IdentityServerConfig cfg)
    {
        builder.Services
            .AddOpenTelemetry()
            .ConfigureResource(ConfigureOpenTelemetryResource)
            .WithMetrics(options =>
            {
                options
                    .AddRuntimeInstrumentation()
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
            }).StartWithHost();

        return builder;
    }
}
