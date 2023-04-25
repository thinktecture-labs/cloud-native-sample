using AdminCli.CliCommands;
using AdminCli.Configuration;
using AdminCli.HttpAccess;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AdminCli.Infrastructure;

public static class DependencyInjection
{
    public static ServiceProvider CreateContainer(ILoggerFactory loggerFactory) =>
        new ServiceCollection().ConfigureServices(loggerFactory)
                               .BuildServiceProvider();

    private static IServiceCollection ConfigureServices(this IServiceCollection services,
                                                        ILoggerFactory loggerFactory) =>
        services.AddLogging(loggerFactory)
                .AddJsonSerializerOptions()
                .AddConfiguration()
                .AddHttpAccess()
                .AddCommandLineApp()
                .AddCliCommands();
}
