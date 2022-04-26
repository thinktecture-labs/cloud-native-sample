using System.IO.Compression;
using Gateway;
using Gateway.Configuration;
using Gateway.TransformProviders;
using Microsoft.AspNetCore.ResponseCompression;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
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
builder.Services.AddHealthChecks();
builder.Services.AddOpenTelemetryTracing(b =>
{ 
    b.AddAspNetCoreInstrumentation()
        .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(typeof(Program).Assembly.GetName().Name))
        .AddHttpClientInstrumentation()
        .AddConsoleExporter()
        .AddZipkinExporter(options =>{
            options.Endpoint = new Uri(cfg.ZipkinEndpoint);
        });
});
var app = builder.Build();
app.UseResponseCompression();
app.UseCors(CorsPolicyName);
app.MapReverseProxy();
app.MapMetrics();
app.UseHttpMetrics();
app.MapHealthChecks("/healthz/readiness");
app.MapHealthChecks("/healthz/liveness");
app.Run();
