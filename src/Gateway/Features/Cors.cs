using Gateway.Configuration;

namespace Gateway.Features;

public static class Cors
{
    public static void Add(WebApplicationBuilder builder, GatewayConfiguration cfg)
    {
        builder.Services.AddCors(options =>
        {
            options.AddPolicy(Constants.CorsPolicyName, builder => { 
                builder
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .WithOrigins(cfg.CorsOrigins); 
            });
            
        });
    }

    public static void Use(WebApplication app)
    {
        app.UseCors(Constants.CorsPolicyName);
    }
}
