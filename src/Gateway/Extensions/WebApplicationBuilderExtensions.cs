using System.Reflection;
using Azure.Monitor.OpenTelemetry.Exporter;
using Microsoft.Extensions.Logging.Console;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Gateway.Configuration;

namespace Microsoft.AspNetCore.Builder;

public static class WebApplicationBuilderExtensions
{
    private static string serviceName = "Gateway";
    private static string appVersion = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "0.0.0";
    private static Action<ResourceBuilder> GetOpenTelemetryResourceBuilder()
    {

        return builder => builder.AddService(serviceName,
            serviceVersion: appVersion,
            serviceInstanceId: Environment.MachineName
        ).Build();
    }

    public static WebApplicationBuilder ConfigureLogging(this WebApplicationBuilder builder, GatewayConfiguration cfg)
    {
        builder.Logging.ClearProviders();
        if (System.Diagnostics.Debugger.IsAttached)
        {
            builder.Logging.AddDebug();
        }
        builder.Logging.AddConsole(options =>
        {
            options.FormatterName = ConsoleFormatterNames.Json;
        });

        builder.Logging.AddOpenTelemetry(options =>
        {
            options.ConfigureResource(GetOpenTelemetryResourceBuilder());
            if (!string.IsNullOrWhiteSpace(cfg.ApplicationInsightsConnectionString))
            {
                Console.WriteLine("Logs: Sending to Azure Monitor");
                options.AddAzureMonitorLogExporter(o => o.ConnectionString = cfg.ApplicationInsightsConnectionString);
            }

        });
        return builder;
    }

    public static WebApplicationBuilder ConfigureTracing(this WebApplicationBuilder builder, GatewayConfiguration cfg)
    {
        builder.Services.AddOpenTelemetryTracing(options =>
        {
            options.ConfigureResource(GetOpenTelemetryResourceBuilder())
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
        });
        return builder;
    }

    public static WebApplicationBuilder ConfigureMetrics(this WebApplicationBuilder builder, GatewayConfiguration cfg)
    {
        builder.Services.AddOpenTelemetryMetrics(options =>
        {
            options.ConfigureResource(GetOpenTelemetryResourceBuilder())
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
        });
        return builder;
    }
}
