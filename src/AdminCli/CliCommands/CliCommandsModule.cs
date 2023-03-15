using AdminCli.CliCommands.Environment;
using AdminCli.CliCommands.Products;
using Microsoft.Extensions.DependencyInjection;

namespace AdminCli.CliCommands;

public static class CliCommandsModule
{
    public static IServiceCollection AddCliCommands(this IServiceCollection services) =>
        services.AddEnvironmentCommands()
                .AddProductCommands();
}
