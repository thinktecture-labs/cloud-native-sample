using System.IO.Compression;
using Gateway.Configuration;
using Microsoft.AspNetCore.ResponseCompression;

namespace Gateway.Features;

public static class ResponseCompression
{
    public static void Add(WebApplicationBuilder builder)
    {
        builder.Services.AddResponseCompression(options =>
        {
            options.EnableForHttps = true;
            options.Providers.Clear();
            options.Providers.Add(new GzipCompressionProvider(new GzipCompressionProviderOptions
            {
                Level = CompressionLevel.Fastest
            }));
        });
    }

    public static void Use(WebApplication app)
    {
        app.UseResponseCompression();
    }

}
