using Microsoft.OpenApi.Models;
using ProductsService.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IProductsRepository, FakeProductsRepository>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger(/*c =>
            {
                c.PreSerializeFilters.Add((swaggerDoc, httpRequest) =>
                {
                    if (!httpRequest.Headers.ContainsKey("X-Forwarded-Host")) return;
                    var basePath = "products";
                    var serverUrl = $"{httpRequest.Scheme}://{httpRequest.Headers["X-Forwarded-Host"]}/{basePath}";
                    swaggerDoc.Servers = new List<OpenApiServer> { new OpenApiServer { Url = serverUrl } };
                });
            }*/);       
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();
app.Run();
