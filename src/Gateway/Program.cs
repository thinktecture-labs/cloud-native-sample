using Gateway.Configuration;
using Gateway.TransformProviders;
using Microsoft.OpenApi.Models;

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
builder.ConfigureLogging(cfg);
// traces
builder.ConfigureTracing(cfg);
// metrics
builder.ConfigureMetrics(cfg);

var logger = builder.Logging.Services.BuildServiceProvider().GetRequiredService<ILogger<Program>>();

// gateway features
Gateway.Features.ResponseCompression.Add(builder);
Gateway.Features.Cors.Add(builder, cfg);
Gateway.Features.HeaderPropagation.Add(builder);
logger.LogInformation("Middlewares: HTTP Header Propagation activated for {HeaderNames}", string.Join(",",
Gateway.Features.HeaderPropagation.PropagatedHeaders));
Gateway.Features.RateLimiting.Add(builder, cfg);

// Reverse Proxy
logger.LogInformation("ReverseProxy: Loading Reverse Proxy configuration from {Section}", cfg.ConfigSection);
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection(cfg.ConfigSection))
    .AddTransforms<DaprTransformProvider>();
logger.LogInformation("ReverseProxy: Custom {TypeName} Transform has been registered", nameof(DaprTransformProvider));


builder.Services.AddControllers();

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

Gateway.Features.ResponseCompression.Use(app);
logger.LogInformation(" - Response Compression activated");
Gateway.Features.Cors.Use(app);
logger.LogInformation(" - CORS activated");
Gateway.Features.HeaderPropagation.Use(app);
logger.LogInformation(" - HTTP Header propagation activated");
app.MapHealthChecks("/healthz/readiness");
logger.LogInformation(" - HealthProbe (readiness) activated");
if (cfg.ExposePrometheusMetrics)
{
    app.UseOpenTelemetryPrometheusScrapingEndpoint();
    logger.LogInformation(" - Prometheus Scraping activated");
}
app.UseRateLimiter();
logger.LogInformation(" - Rate Limiter activated");
app.MapHealthChecks("/healthz/liveness");
logger.LogInformation(" - HealthProbe (liveness) activated");
app.MapReverseProxy();
logger.LogInformation(" - Reverse Proxy activated");
app.MapControllers();
logger.LogInformation(" - API Controllers activated");
logger.LogInformation("All middlewares activated");
app.Run();
