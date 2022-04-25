using Dapr.Client;
using OrdersService.Configuration;

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
builder.Services.AddScoped<DaprClient>(_ => new DaprClientBuilder().Build()!);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHealthChecks();

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/healthz/readiness");
app.MapHealthChecks("/healthz/liveness");
app.Run();
