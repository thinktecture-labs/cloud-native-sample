using Microsoft.Extensions.DependencyInjection;

namespace AdminCli.CliCommands.Environment;

public static class EnvironmentCommandsModule
{
    public static IServiceCollection AddEnvironmentCommands(this IServiceCollection services) =>
        services.AddSingleton<EnvironmentCommand>()
                .AddSingleton<ShowEnvironmentCommand>()
                .AddSingleton<SetEnvironmentCommand>();
}
