using System.IO.Compression;
using Gateway;
using Gateway.Configuration;
using Gateway.TransformProviders;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Logging.Console;
using Microsoft.OpenApi.Models;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);

var cfg = new GatewayConfiguration();
var cfgSection = builder.Configuration.GetSection(GatewayConfiguration.SectionName);

if (cfgSection == null || !cfgSection.Exists())
{
    throw new ApplicationException(
        $"Could not find Gateway configuration. Please ensure a '{GatewayConfiguration.SectionName}' exists");
}
cfgSection.Bind(cfg);
builder.Services.AddSingleton(cfg);

// logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole(options =>
{
    options.FormatterName = cfg.ConsoleFormatterName;
});

var logger = builder.Logging.Services.BuildServiceProvider().GetRequiredService<ILogger<Program>>();

// traces
if (string.IsNullOrWhiteSpace(cfg.TraceEndpoint))
{
    logger.LogWarning("TraceEndpoint not configured. Traces will not be exported!");
}

if (!string.IsNullOrWhiteSpace(cfg.TraceEndpoint))
{
    builder.Services.AddOpenTelemetryTracing(options =>
    {
        logger.LogInformation("Tracing: Traces will be exported to {TraceSystem} at {TraceEndpoint}", cfg.TraceSystem, cfg.TraceEndpoint);
        options.SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(Constants.ServiceName)).AddAspNetCoreInstrumentation();
        
        logger.LogInformation("Tracing: Service Name is set to {ServiceName}", Constants.ServiceName);
        switch (cfg.TraceSystem)
        {
            case TraceSystem.Zipkin:
                options.AddZipkinExporter(config => { config.Endpoint = new Uri(cfg.TraceEndpoint); });
                break;
            case TraceSystem.AzureMonitor:
                logger.LogWarning("Azure SDK is currently not compatible with OTel SDK... Will not write traces to Azure Monitor");
                
                // options.AddAzureMonitorTraceExporter(config => { config.ConnectionString = cfg.TraceEndpoint; });
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    });

// metrics
    builder.Services.AddOpenTelemetryMetrics(options =>
    {
        options.ConfigureResource(rb => { rb.AddService(Constants.ServiceName); })
            .AddRuntimeInstrumentation()
            .AddHttpClientInstrumentation()
            .AddAspNetCoreInstrumentation()
            .AddPrometheusExporter();
        logger.LogInformation("Metrics: Service Name is set to {ServiceName}", Constants.ServiceName);
    });

    builder.Services.AddResponseCompression(options =>
    {
        options.EnableForHttps = true;
        options.Providers.Clear();
        options.Providers.Add(new GzipCompressionProvider(new GzipCompressionProviderOptions
        {
            Level = CompressionLevel.Fastest
        }));
        logger.LogInformation("Middlewares: ResponseCompression is activated");
    });
}

builder.Services.AddCors(options =>
{
    options.AddPolicy(Constants.CorsPolicyName, builder => { 
        builder
            .AllowAnyHeader()
            .AllowAnyMethod()
            .WithOrigins(cfg.CorsOrigins); 
    });
    logger.LogInformation("Middlewares: CORS is allowed for {Origins} under policy {PolicyName}", string.Join(",",cfg.CorsOrigins), Constants.CorsPolicyName);
    
});

logger.LogInformation("ReverseProxy: Loading Reverse Proxy configuration from {Section}", cfg.ConfigSection);
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection(cfg.ConfigSection))
    .AddTransforms<DaprTransformProvider>();
logger.LogInformation("ReverseProxy: Custom {TypeName} Transform has been registered", nameof(DaprTransformProvider));

builder.Services.AddControllers();

string[] propagatedHeaders = new [] { "Authorization "};
builder.Services.AddHeaderPropagation(options => {
    propagatedHeaders.ToList().ForEach(h=> {
        options.Headers.Add(h);
    });
});
builder.Services.AddHttpClient(Constants.HttpClientName)
    .AddHeaderPropagation();
logger.LogInformation("Middlewares: HTTP Header Propagation activated for {HeaderNames}", string.Join(",", propagatedHeaders));


builder.Services.AddSwaggerGen(c =>
{
    c.EnableAnnotations();
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Order Monitor Service",
        Description = "Fairly simple .NET API to interact with orders for monitoring",
        Contact = new OpenApiContact
        {
            Name = "Thinktecture AG",
            Email = "info@thinktecture.com",
            Url = new Uri("https://thinktecture.com")
        }
    });
});

builder.Services.AddHealthChecks();

var app = builder.Build();

logger.LogInformation("Activating Middlewares");
app.UseSwagger();
app.UseSwaggerUI();
logger.LogInformation(" - Swagger and SwaggerUI activated");
app.UseResponseCompression();
logger.LogInformation(" - Response Compression activated");
app.UseCors(Constants.CorsPolicyName);
logger.LogInformation(" - CORS activated");
app.UseHeaderPropagation();
logger.LogInformation(" - HTTP Header propagation activated");
app.MapReverseProxy();
logger.LogInformation(" - Reverse Proxy activated");
app.MapControllers();
logger.LogInformation(" - API Controllers activated");
app.MapHealthChecks("/healthz/readiness");
logger.LogInformation(" - HealthProbe (readiness) activated");
app.MapHealthChecks("/healthz/liveness");
logger.LogInformation(" - HealthProbe (liveness) activated");
app.UseOpenTelemetryPrometheusScrapingEndpoint();
logger.LogInformation(" - Prometheus Scraping activated");
logger.LogInformation("All middlewares activated"); 
app.Run();
