using NotificationService;
using NotificationService.Configuration;
using Prometheus;

var builder = WebApplication.CreateBuilder(args);

var cfg = new NotificationServiceConfiguration();
var cfgSection = builder.Configuration.GetSection(NotificationServiceConfiguration.SectionName);
if (cfgSection == null || !cfgSection.Exists())
{
    throw new ApplicationException($"Configuration not found. Please specify the {NotificationServiceConfiguration.SectionName} section");
}

builder.Services.AddSingleton(cfg);
// Add services to the container.
builder.Services.AddSignalR();
builder.Services.AddHealthChecks();
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseSwagger();
app.UseSwaggerUI();
app.UseAuthorization();
app.MapHub<NotificationHub>("/notifications");
app.MapMetrics();
app.UseHttpMetrics();
app.MapHealthChecks("/healthz/readiness");
app.MapHealthChecks("/healthz/liveness");
app.MapControllers();

app.Run();
