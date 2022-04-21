using Gateway;
using OpenTelemetry.Instrumentation.AspNetCore;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"))
    .AddTransforms<DaprRequestTransformProvider>();
builder.Services.AddHealthChecks();
builder.Services.AddOpenTelemetryTracing(b =>
{ 
    b.AddAspNetCoreInstrumentation()
        .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(typeof(Program).Assembly.GetName().Name))
        .AddHttpClientInstrumentation()
        .AddConsoleExporter()
        .AddZipkinExporter(cfg =>{
            // todo: extract uri to configuration
            cfg.Endpoint = new Uri("http://localhost:9412/api/v2/spans");
        });
});
var app = builder.Build();
app.MapReverseProxy();
app.MapHealthChecks("/healthz");
app.Run();
