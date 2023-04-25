using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;

namespace AdminCli.Infrastructure;

public static class Json
{
    public static IServiceCollection AddJsonSerializerOptions(this IServiceCollection services) =>
        services.AddSingleton(CreateOptions());
    
    private static JsonSerializerOptions CreateOptions() =>
        new ()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false,
            PropertyNameCaseInsensitive = true
        };
}
