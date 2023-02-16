using Microsoft.Extensions.Logging.Console;
using Microsoft.OpenApi.Models;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using ProductsService.Configuration;
using ProductsService.Data.Repositories;
using ProductsService.Migrations;

var builder = WebApplication.CreateBuilder(args);

var cfg = new ProductsServiceConfiguration();
var cfgSection = builder.Configuration.GetSection(ProductsServiceConfiguration.SectionName);

if (cfgSection == null || !cfgSection.Exists())
{
    throw new ApplicationException(
        $"Could not find service config. Please provide a '{ProductsServiceConfiguration.SectionName}' config section");
}
cfgSection.Bind(cfg);
builder.Services.AddSingleton(cfg);
// logging
builder.ConfigureLogging(cfg);
// tracing
builder.ConfigureTracing(cfg);
// metrics
builder.ConfigureMetrics(cfg);

// Run Database Migrations
// This is a poor-man's migration system. It's not very robust, but it's good enough for this demo.
// In a real-world scenario, you'd probably want to use a proper migration system like FluentMigrator.
builder.RunMigrations(cfg);

// Configure AuthN
builder.ConfigureAuthN(cfg);
// Configure AuthZ
builder.ConfigureAuthZ(cfg);

builder.Services.AddScoped<IProductsRepository>(services =>
{
    var cfg = services.GetRequiredService<ProductsServiceConfiguration>();
    if (string.IsNullOrWhiteSpace(cfg.ConnectionString))
    {
        return new InMemoryProductsRepository(services.GetRequiredService<ILogger<InMemoryProductsRepository>>());
    }
    return new ProductsRepository(cfg, services.GetRequiredService<ILogger<ProductsRepository>>());
});

builder.Services.AddHealthChecks();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.EnableAnnotations();
    c.SwaggerDoc("v1", new OpenApiInfo()
    {
        Version = "v1",
        Title = "Products Service",
        Description = "Fairly simple .NET API to interact with product data",
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
    Console.WriteLine("Registering Prometheus scraping endpoint");
    app.UseOpenTelemetryPrometheusScrapingEndpoint();
}

app.MapHealthChecks("/healthz/readiness");
app.MapHealthChecks("/healthz/liveness");

app.Run();
