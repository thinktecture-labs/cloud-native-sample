using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using NotificationService;
using NotificationService.Configuration;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

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
}

// metrics
builder.Services.AddOpenTelemetryMetrics(options =>
{
    options.ConfigureResource(rb =>
        {
            rb.AddService(Constants.ServiceName);
        })
        .AddRuntimeInstrumentation()
        .AddHttpClientInstrumentation()
        .AddAspNetCoreInstrumentation()
        .AddPrometheusExporter();
    logger.LogInformation("Metrics: Service Name is set to {ServiceName}", Constants.ServiceName);
}); 


// Configure AuthN
var notificationHubEndpoint = "/notifications/notificationHub";

builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.Authority = cfg.IdentityServer.Authority;
        options.RequireHttpsMetadata = cfg.IdentityServer.RequireHttpsMetadata;
        if(!string.IsNullOrWhiteSpace(cfg.IdentityServer.MetadataAddress))
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
                    (path.StartsWithSegments(notificationHubEndpoint)))
                {
                    // Read the token out of the query string
                    context.Token = accessToken;
                }
                return Task.CompletedTask;
            }
        };
    });
logger.LogInformation("Authentication configured for {Authority} (require HTTPs: {requireHttps}) Metadata Address {MetadataAddress}", 
    cfg.IdentityServer.Authority, 
    cfg.IdentityServer.RequireHttpsMetadata, 
    cfg.IdentityServer.MetadataAddress);

// Configure AuthZ
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(Constants.AuthorizationPolicyName, policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireClaim(cfg.Authorization.RequiredClaimName, cfg.Authorization.RequiredClaimValue);
    });
});
logger.LogInformation("Authorization configured with Policy {AuthzPolicyName}: RequiredClaim: {Name} {Value}",
    Constants.AuthorizationPolicyName,
    cfg.Authorization.RequiredClaimName,
    cfg.Authorization.RequiredClaimValue);

builder.Services.AddSignalR();
builder.Services.AddHealthChecks();
builder.Services.AddControllers();

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
app.MapHub<NotificationHub>(notificationHubEndpoint)
    .RequireAuthorization("ApiScope");
logger.LogInformation(" - SignalR Hubs activated");
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
