using Microsoft.Extensions.Logging.Console;
using Microsoft.OpenApi.Models;
using PriceDropNotifier.Configuration;


var builder = WebApplication.CreateBuilder(args);

var cfg = new PriceDropNotifierServiceConfiguration();
var cfgSection = builder.Configuration.GetSection(PriceDropNotifierServiceConfiguration.SectionName);

if (cfgSection == null || !cfgSection.Exists())
{
    throw new ApplicationException(
        $"Could not find service config. Please provide a '{PriceDropNotifierServiceConfiguration.SectionName}' section in your appsettings.json file."
    );
}

cfgSection.Bind(cfg);
builder.Services.AddSingleton(cfg);

builder.ConfigureLogging(cfg)
    .ConfigureTracing(cfg)
    .ConfigureMetrics(cfg)
    .ConfigureAuthN(cfg)
    .ConfigureAuthZ(cfg)
    .Services.AddHealthChecks()
    .Services.AddDaprClient();

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(config =>
{
    config.EnableAnnotations();
    config.SwaggerDoc("v1", new OpenApiInfo()
    {
        Version = "v1",
        Title = "PriceDrop Service",
        Description = "Fairly simple .NET API to watch for price drops.",
        Contact = new OpenApiContact
        {
            Name = "Thinktecture AG",
            Email = "info@thinktecture.com",
            Url = new Uri("https://thinktecture.com")
        }
    });
});

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers()
    .RequireAuthorization("RequiresApiScope");

if (cfg.ExposePrometheusMetrics)
{
    app.UseOpenTelemetryPrometheusScrapingEndpoint();
}

app.MapHealthChecks("/healthz/readiness");
app.MapHealthChecks("/healthz/liveness");
app.Run();

