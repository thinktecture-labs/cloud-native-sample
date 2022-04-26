using Microsoft.OpenApi.Models;
using ProductsService.Configuration;
using ProductsService.Repositories;
using Prometheus;

var builder = WebApplication.CreateBuilder(args);

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


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseAuthorization();
app.MapControllers();
app.MapMetrics();
app.UseHttpMetrics();
app.MapHealthChecks("/healthz/readiness");
app.MapHealthChecks("/healthz/liveness");
app.Run();
