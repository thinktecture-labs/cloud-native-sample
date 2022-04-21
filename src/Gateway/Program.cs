using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddReverseProxy().LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

builder.Services.AddOpenTelemetryTracing(b =>
{
    b.AddAspNetCoreInstrumentation()
        .AddJaegerExporter();
});
var app = builder.Build();
app.MapReverseProxy();

app.Run();
