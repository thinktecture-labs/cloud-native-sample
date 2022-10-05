using System.IO.Compression;
using Gateway.Configuration;
using Gateway.TransformProviders;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.OpenApi.Models;
using Prometheus;


var builder = WebApplication.CreateBuilder(args);

const string CorsPolicyName = "GatewayPolicy";

var cfg = new GatewayConfiguration();
var cfgSection = builder.Configuration.GetSection(GatewayConfiguration.SectionName);

if (cfgSection == null || !cfgSection.Exists())
{
    throw new ApplicationException($"Could not find Gateway configuration. Please ensure a '{GatewayConfiguration.SectionName}' exists");
}
else
{
    cfgSection.Bind(cfg);
}

builder.Services.AddSingleton(cfg);
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
    options.Providers.Clear();
    options.Providers.Add(new GzipCompressionProvider(new GzipCompressionProviderOptions
    {
        Level = CompressionLevel.Fastest
    }));
});

builder.Services.AddCors(options =>
{
    options.AddPolicy(CorsPolicyName, b =>
    {
        b.AllowAnyHeader().AllowAnyMethod().WithOrigins(cfg.CorsOrigins);
    });
});

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection(cfg.ConfigSection)) 
    .AddTransforms<DaprTransformProvider>();

builder.Services.AddControllers();

builder.Services.AddHeaderPropagation(o =>
{
    o.Headers.Add("Authorization");
});
builder.Services.AddHttpClient("ordermonitor").AddHeaderPropagation();


builder.Services.AddSwaggerGen(c =>
{
    c.EnableAnnotations();
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Order Monitor Service",
        Description = "Fairly simple .NET API to interact with orders for monitoring",
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

app.UseResponseCompression();
app.UseCors(CorsPolicyName);
app.MapReverseProxy();

app.MapMetrics();
app.UseHttpMetrics();

app.MapControllers();

app.MapHealthChecks("/healthz/readiness");
app.MapHealthChecks("/healthz/liveness");

app.Run();
