using System;
using System.Threading.Tasks;
using AdminCli.CliCommands.Environment;
using AdminCli.Configuration;
using AdminCli.Infrastructure;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.DependencyInjection;

namespace AdminCli;

public static class Program
{
    public static async Task<int> Main(string[] args)
    {
        var logger = Logging.CreateLogger();
        try
        {
            await using var container = DependencyInjection.CreateContainer();
            var configurationManager = container.GetRequiredService<IConfigurationManager>();
            configurationManager.SetLogLevelFromAppSettings();
            container.GetRequiredService<ShowEnvironmentCommand>().Execute();

            var app = container.GetRequiredService<CommandLineApplication>();
            return await app.ExecuteAsync(args);
        }
        catch (Exception exception)
        {
            logger.Fatal(exception, "Admin CLI encountered an error");
            return 1;
        }
    }
}
