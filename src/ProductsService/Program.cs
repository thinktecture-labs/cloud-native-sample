using ProductsService.Configuration;
using ProductsService.Repositories;

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
builder.Services.AddScoped<IProductsRepository, FakeProductsRepository>();
builder.Services.AddHealthChecks();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
