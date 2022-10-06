using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Logging.Console;
using Microsoft.IdentityModel.Tokens;
using NotificationService;
using NotificationService.Configuration;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

const string ServiceName = "NotificationService";

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
    options.FormatterName = ConsoleFormatterNames.Json;
});

//traces
if (string.IsNullOrWhiteSpace(cfg.ZipkinEndpoint))
{
    throw new ApplicationException("Zipkin Endpoint not provided");
}

builder.Services.AddOpenTelemetryTracing(options =>
{
    options.SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(ServiceName))
        .AddAspNetCoreInstrumentation()
        .AddZipkinExporter(config =>
        {
            config.Endpoint = new Uri(cfg.ZipkinEndpoint);
        });
});

// metrics
builder.Services.AddOpenTelemetryMetrics(options =>
{
    options.ConfigureResource(rb =>
        {
            rb.AddService(ServiceName);
        })
        .AddRuntimeInstrumentation()
        .AddHttpClientInstrumentation()
        .AddAspNetCoreInstrumentation()
        .AddPrometheusExporter();
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

// Configure AuthZ
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ApiScope", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireClaim(cfg.Authorization.RequiredClaimName, cfg.Authorization.RequiredClaimValue);
    });
});

builder.Services.AddSignalR();
builder.Services.AddHealthChecks();
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthentication();
app.UseAuthorization();

app.MapHub<NotificationHub>(notificationHubEndpoint)
    .RequireAuthorization("ApiScope");

app.MapHealthChecks("/healthz/readiness");
app.MapHealthChecks("/healthz/liveness");

app.MapControllers();

app.UseOpenTelemetryPrometheusScrapingEndpoint();

app.Run();
