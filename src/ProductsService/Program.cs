using Microsoft.Extensions.Logging.Console;
using Microsoft.OpenApi.Models;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using ProductsService.Configuration;
using ProductsService.Data.Repositories;

const string ServiceName = "ProductsService";

var builder = WebApplication.CreateBuilder(args);

// logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole(options =>
{
    options.FormatterName = ConsoleFormatterNames.Json;
});


var zipkinEndpoint = builder.Configuration.GetValue<string>("ZipkinEndpoint");
if (string.IsNullOrWhiteSpace(zipkinEndpoint))
{
    throw new ApplicationException("Zipkin Endpoint not provided");
}

//traces
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

var cfg = new ProductsServiceConfiguration();
var cfgSection = builder.Configuration.GetSection(ProductsServiceConfiguration.SectionName);

if (cfgSection == null || !cfgSection.Exists())
{
    throw new ApplicationException(
        $"Could not find service config. Please provide a '{ProductsServiceConfiguration.SectionName}' config section");
}
else
{
    cfgSection.Bind(cfg);
}

builder.Services.AddSingleton(cfg);
builder.Services.AddScoped<IProductsRepository>(serviceProvider =>
{
    var c = serviceProvider.GetRequiredService<ProductsServiceConfiguration>();

    if (c.UseFakeImplementation)
    {
        var l = serviceProvider.GetRequiredService<ILogger<FakeProductsRepository>>();
        return new FakeProductsRepository(l);
    }
    throw new NotFiniteNumberException("No live implementation here yet...");
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

app.UseAuthorization();

app.MapControllers();

app.UseOpenTelemetryPrometheusScrapingEndpoint();

app.MapHealthChecks("/healthz/readiness");
app.MapHealthChecks("/healthz/liveness");

app.Run();
