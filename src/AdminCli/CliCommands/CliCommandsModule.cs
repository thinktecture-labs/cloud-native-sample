using Microsoft.Extensions.DependencyInjection;

namespace AdminCli.CliCommands;

public static class CliCommandsModule
{
    public static IServiceCollection AddCliCommands(this IServiceCollection services) =>
        services.AddSingleton<ListProductsCommand>()
                .AddSingleton<DropPriceCommand>()
                .AddSingleton<ChangeLogLevelCommand>()
                .AddSingleton<TargetLocalCommand>()
                .AddSingleton<TargetCloudCommand>();
}
