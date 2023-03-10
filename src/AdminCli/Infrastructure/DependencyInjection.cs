using AdminCli.CliCommands;
using AdminCli.Configuration;
using AdminCli.HttpAccess;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace AdminCli.Infrastructure;

public static class DependencyInjection
{
    public static ServiceProvider CreateContainer() =>
        new ServiceCollection().ConfigureServices()
                               .BuildServiceProvider();

    private static IServiceCollection ConfigureServices(this IServiceCollection services) =>
        services.AddLogging(options => options.AddSerilog())
                .AddJsonSerializerOptions()
                .AddConfiguration()
                .AddHttpAccess()
                .AddCommandLineApp()
                .AddCliCommands();
}
