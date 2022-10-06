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
// logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole(options =>
{
    options.FormatterName = ConsoleFormatterNames.Json;
});


//traces
var zipkinEndpoint = builder.Configuration.GetValue<string>("ZipkinEndpoint");
if (string.IsNullOrWhiteSpace(zipkinEndpoint))
{
    throw new ApplicationException("Zipkin Endpoint not provided");
}

builder.Services.AddOpenTelemetryTracing(options =>
{
    options.SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(ServiceName))
        .AddAspNetCoreInstrumentation()
        .AddZipkinExporter(config =>
        {
            config.Endpoint = new Uri(zipkinEndpoint);
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

var cfg = new NotificationServiceConfiguration();
var cfgSection = builder.Configuration.GetSection(NotificationServiceConfiguration.SectionName);

if (cfgSection == null || !cfgSection.Exists())
{
    throw new ApplicationException($"Configuration not found. Please specify the {NotificationServiceConfiguration.SectionName} section");
}

builder.Services.AddSingleton(cfg);

// Configure AuthN
var notificationHubEndpoint = "/notifications/notificationHub";

builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        //todo: check if we can put authority in DI
        options.Authority = builder.Configuration.GetValue<string>("Authority");
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false,
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
        policy.RequireClaim("scope", "sample");
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

app.MapHub<NotificationHub>(notificationHubEndpoint);

app.MapHealthChecks("/healthz/readiness");
app.MapHealthChecks("/healthz/liveness");

app.MapControllers();

app.UseOpenTelemetryPrometheusScrapingEndpoint();

app.Run();
