using Dapr.Client;
using Microsoft.OpenApi.Models;
using OrdersService.Configuration;
using Microsoft.EntityFrameworkCore;
using OrdersService.Data;
using OrdersService.Data.Repositories;

var builder = WebApplication.CreateBuilder(args);

var cfg = new OrdersServiceConfiguration();
var cfgSection = builder.Configuration.GetSection(OrdersServiceConfiguration.SectionName);

if (!cfgSection.Exists())
{
    throw new ApplicationException(
        $"Could not find service config. Please provide a '{OrdersServiceConfiguration.SectionName}' config section");
}
cfgSection.Bind(cfg);
builder.Services.AddSingleton(cfg);

// logging
builder.ConfigureLogging(cfg);
// tracing
builder.ConfigureTracing(cfg);
// metrics
builder.ConfigureMetrics(cfg);

// Configure AuthN
builder.ConfigureAuthN(cfg);
// Configure AuthZ
builder.ConfigureAuthZ(cfg);

builder.Services.AddDbContext<OrdersServiceContext>(options =>
{
    // Depending on whether OrdersService:ConnectionString is set,
    // we target either an in-memory provider or a real SQL Server database
    if (cfg.TryGetConnectionString(out var connectionString))
        options.UseSqlServer(connectionString);
    else
        options.UseInMemoryDatabase(databaseName: "OrdersService");
});

builder.Services.AddScoped<IOrdersRepository, OrdersRepository>();
builder.Services.AddScoped<DaprClient>(_ => new DaprClientBuilder().Build()!);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.EnableAnnotations();
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Orders Service",
        Description = "Fairly simple .NET API to interact with orders",
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

// Apply migrations on startup. Microsoft does not recommend this approach in real projects.
// You should use generated SQL scripts that are applied manually instead.
// For the sake of simplicity, we call Migrate here.
// See: https://learn.microsoft.com/en-us/ef/core/managing-schemas/migrations/applying
if (cfg.CheckIfConnectionStringIsPresent())
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<OrdersServiceContext>();
    dbContext.Database.Migrate();
}

app.UseSwagger();
app.UseSwaggerUI();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers()
    .RequireAuthorization("RequiresApiScope");

app.MapHealthChecks("/healthz/readiness");
app.MapHealthChecks("/healthz/liveness");

if (cfg.ExposePrometheusMetrics)
{
    Console.WriteLine("Registering Prometheus scraping endpoint");
    app.UseOpenTelemetryPrometheusScrapingEndpoint();
}

app.Run();
