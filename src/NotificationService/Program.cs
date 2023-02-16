using System.Text.Json;
using Dapr;
using NotificationService;
using NotificationService.Configuration;

var builder = WebApplication.CreateBuilder(args);

var cfg = new NotificationServiceConfiguration();
var cfgSection = builder.Configuration.GetSection(NotificationServiceConfiguration.SectionName);

if (cfgSection == null || !cfgSection.Exists())
{
    throw new ApplicationException($"Configuration not found. Please specify the {NotificationServiceConfiguration.SectionName} section");
}
cfgSection.Bind(cfg);
builder.Services.AddSingleton(cfg);

// logging
builder.ConfigureLogging(cfg);
var logger = builder.Logging.Services.BuildServiceProvider().GetRequiredService<ILogger<Program>>();

// traces
builder.ConfigureTracing(cfg);
// metrics
builder.ConfigureMetrics(cfg);
// Configure AuthN
builder.ConfigureAuthN(cfg);

// Configure AuthZ
builder.ConfigureAuthZ(cfg);


builder.Services.AddSignalR();
builder.Services.AddHealthChecks();
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
logger.LogInformation("Activating Middlewares");
app.UseSwagger();
app.UseSwaggerUI();
logger.LogInformation(" - Swagger and SwaggerUI activated");
app.UseAuthentication();
logger.LogInformation(" - Authentication activated");
app.UseAuthorization();
logger.LogInformation(" - Authorization activated");
app.MapHub<NotificationHub>(Constants.NotificationHubEndpoint)
    .RequireAuthorization("RequiresApiScope");
logger.LogInformation(" - SignalR Hubs activated");
app.MapControllers();
logger.LogInformation(" - API Controllers activated");
app.UseCloudEvents();
app.MapSubscribeHandler();
logger.LogInformation(" - Dapr Subscriptions activated with CloudEvents");
app.MapHealthChecks("/healthz/readiness");
logger.LogInformation(" - HealthProbe (readiness) activated");
app.MapHealthChecks("/healthz/liveness");
logger.LogInformation(" - HealthProbe (liveness) activated");
if (cfg.ExposePrometheusMetrics)
{
    app.UseOpenTelemetryPrometheusScrapingEndpoint();
    logger.LogInformation(" - Prometheus Scraping activated");
}
logger.LogInformation("All middlewares activated");

app.Run();
