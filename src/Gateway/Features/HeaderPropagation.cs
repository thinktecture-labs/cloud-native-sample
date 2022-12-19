namespace Gateway.Features;

public static class HeaderPropagation
{
    public static string[] PropagatedHeaders = new [] { "Authorization" };
    public static void Add(WebApplicationBuilder builder)
    {
        builder.Services.AddHeaderPropagation(options => {
            PropagatedHeaders.ToList().ForEach(h=> {
                options.Headers.Add(h);
            });
        });

        builder.Services.AddHttpClient(Constants.HttpClientName)
            .AddHeaderPropagation(); 
    }

    public static void Use(WebApplication app)
    {
        app.UseHeaderPropagation();
    }
}
