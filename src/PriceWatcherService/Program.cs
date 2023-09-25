using Microsoft.OpenApi.Models;
using PriceWatcher.Configuration;
using PriceWatcher.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Create and register strongly-typed config object
var cfg = new PriceWatcherServiceConfiguration();
var cfgSection = builder.Configuration.GetSection(PriceWatcherServiceConfiguration.SectionName);
if (!cfgSection.Exists())
{
    throw new ApplicationException(
        $"Could not find service config. Please provide a '{PriceWatcherServiceConfiguration.SectionName}' section in your appsettings.json file."
    );
}
cfgSection.Bind(cfg);
builder.Services.AddSingleton(cfg);

// Add further services to the container.
builder
   .ConfigureAuthN(cfg)
   .ConfigureAuthZ(cfg);

builder.Services.AddSingleton<IPriceWatcherRepository, PriceWatcherRepository>();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(config => {
    config.EnableAnnotations();
    config.SwaggerDoc("v1", new OpenApiInfo()
    {
        Version = "v1",
        Title = "PriceWatcher Service",
        Description = "Fairly simple .NET API to watch for price drops.",
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

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers().RequireAuthorization("RequiresApiScope");

app.Run();
