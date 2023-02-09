using System.Threading.RateLimiting;
using Gateway.Configuration;
using Microsoft.AspNetCore.RateLimiting;

namespace Gateway.Features;

public static class RateLimiting
{
    public static void Add(WebApplicationBuilder builder, GatewayConfiguration cfg)
    {
        builder.Services.AddRateLimiter(options =>
             {
                options.RejectionStatusCode = 429;
                 options.AddFixedWindowLimiter("CloudNativeSamplePolicy", opt =>
                {
                    opt.PermitLimit = cfg.LimitRequestsPerMinute;
                    opt.Window = TimeSpan.FromMinutes(1);
                });
             });
    }
}
