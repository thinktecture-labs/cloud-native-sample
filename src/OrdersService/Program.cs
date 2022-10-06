using Dapr.Client;
using Microsoft.OpenApi.Models;
using OrdersService.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Console;
using OrdersService.Data;
using OrdersService.Data.Repositories;
using Microsoft.IdentityModel.Tokens;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

const string ServiceName = "OrdersService";
var builder = WebApplication.CreateBuilder(args);

var cfg = new OrdersServiceConfiguration();
var cfgSection = builder.Configuration.GetSection(OrdersServiceConfiguration.SectionName);

if (cfgSection == null || !cfgSection.Exists())
{
    throw new ApplicationException(
        $"Could not find service config. Please provide a '{OrdersServiceConfiguration.SectionName}' config section");
}
else
{
    cfgSection.Bind(cfg);
}

builder.Services.AddSingleton(cfg);


// logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole(options =>
{
    options.FormatterName = ConsoleFormatterNames.Json;
});


if (string.IsNullOrWhiteSpace(cfg.ZipkinEndpoint))
{
    throw new ApplicationException("Zipkin Endpoint not provided");
}
//traces
builder.Services.AddOpenTelemetryTracing(options =>
{
    options.SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(ServiceName))
        .AddAspNetCoreInstrumentation()
        .AddZipkinExporter(options =>
        {
            options.Endpoint = new Uri(cfg.ZipkinEndpoint);
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
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        
        options.Authority = cfg.IdentityServer.Authority;
        options.RequireHttpsMetadata = cfg.IdentityServer.RequireHttpsMetadata;
        
        if (!string.IsNullOrWhiteSpace(cfg.IdentityServer.MetadataAddress))
        {
            options.MetadataAddress = cfg.IdentityServer.MetadataAddress;
        }
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false,
            ValidIssuer = cfg.IdentityServer.Authority  
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

builder.Services.AddDbContext<OrdersServiceContext>(options =>
    options.UseInMemoryDatabase(databaseName: "OrdersService"));

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

app.UseSwagger();
app.UseSwaggerUI();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers()
    .RequireAuthorization("ApiScope");

app.MapHealthChecks("/healthz/readiness");
app.MapHealthChecks("/healthz/liveness");

app.UseOpenTelemetryPrometheusScrapingEndpoint();

app.Run();
