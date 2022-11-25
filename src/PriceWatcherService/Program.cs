using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Options;
using PriceWatcher.Repositories;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);

const string ServiceName = "PriceWatcher";

// Logging
builder.Logging.ClearProviders();
// always log to STDOUT 
builder.Logging.AddConsole(options =>
{
    options.FormatterName = ConsoleFormatterNames.Json;
});

// instrumentation

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
        .AddZipkinExporter(config => {
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

// Add services to the container.
builder.Services.AddDaprClient();

builder.Services.AddSingleton<IPriceWatcherRepository, PriceWatcherRepository>();
builder.Services.AddControllers();
builder.Services.AddHealthChecks();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

// decide if you want to check just this component or all dependencies too!?
// both are valid...
app.MapHealthChecks("/healthz/readiness");
app.MapHealthChecks("/healthz/liveness");

// expose metrics for prometheus scraping
app.UseOpenTelemetryPrometheusScrapingEndpoint();

app.Run();
