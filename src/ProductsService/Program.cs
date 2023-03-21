using System.Data.SqlClient;
using ProductsService.Configuration;
using ProductsService.Controllers;
using ProductsService.Data.Repositories;
using ProductsService.Data.UnitOfWork;
using ProductsService.OutboxProcessing;

var builder = WebApplication.CreateBuilder(args);

var cfg = new ProductsServiceConfiguration();
var cfgSection = builder.Configuration.GetSection(ProductsServiceConfiguration.SectionName);

if (!cfgSection.Exists())
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

// Configure data access layer
if (string.IsNullOrWhiteSpace(cfg.ConnectionString))
{
    builder.Services
           .AddSingleton<InMemoryProductsRepository>()
           .AddSingleton<IProductsRepository>(c => c.GetRequiredService<InMemoryProductsRepository>())
           .AddUnitOfWorkWithFactory<IPriceDropUnitOfWork, InMemoryPriceDropUnitOfWork>()
           .AddUnitOfWorkWithFactory<IOutboxUnitOfWork, InMemoryOutboxUnitOfWork>(
                unitOfWorkLifetime: ServiceLifetime.Singleton,
                factoryLifetime: ServiceLifetime.Singleton
            );
}
else
{
    builder.Services
           .AddScoped<IProductsRepository, ProductsRepository>()
           .AddTransient(_ => new SqlConnection(cfg.ConnectionString))
           .AddUnitOfWorkWithFactory<IPriceDropUnitOfWork, SqlPriceDropUnitOfWork>()
           .AddUnitOfWorkWithFactory<IOutboxUnitOfWork, SqlOutboxUnitOfWork>(
                unitOfWorkLifetime: ServiceLifetime.Transient,
                factoryLifetime: ServiceLifetime.Singleton
            );
}

// Configure Transactional Outbox
if (cfg.EnableOutboxProcessing)
    builder.Services
           .AddSingleton<OutboxProcessor>()
           .AddHostedService(serviceProvider => serviceProvider.GetRequiredService<OutboxProcessor>())
           .AddSingleton<IOutboxProcessor>(serviceProvider => serviceProvider.GetRequiredService<OutboxProcessor>());
else
    builder.Services
           .AddSingleton<IOutboxProcessor, NullOutboxProcessor>();

// Configure Dapr PubSub
if (cfg.UseFakeEventPublisher)
{
    builder.Services
           .AddSingleton<IEventPublisher, FakeEventPublisher>();
}
else
{
    builder.Services
           .AddSingleton<IEventPublisher, DaprEventPublisher>()
           .AddDaprClient();
}

builder.Services.AddHealthChecks();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.EnableAnnotations();
    c.SwaggerDoc("v1",
                 new()
                 {
                     Version = "v1",
                     Title = "Products Service",
                     Description = "Fairly simple .NET API to interact with product data",
                     Contact = new()
                     {
                         Name = "Thinktecture AG",
                         Email = "info@thinktecture.com",
                         Url = new ("https://thinktecture.com")
                     }
                 });
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

if (cfg.ExposePrometheusMetrics)
{
    Console.WriteLine("Registering Prometheus scraping endpoint");
    app.UseOpenTelemetryPrometheusScrapingEndpoint();
}

app.MapHealthChecks("/healthz/readiness");
app.MapHealthChecks("/healthz/liveness");

app.Run();
