using System;
using System.Threading.Tasks;
using AdminCli.CliCommands.Environment;
using AdminCli.Infrastructure;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AdminCli;

public static class Program
{
    public static async Task<int> Main(string[] args)
    {
        var loggerFactory = Logging.CreateLoggingFactory(args);
        try
        {
            await using var container = DependencyInjection.CreateContainer(loggerFactory);
            container.GetRequiredService<ShowEnvironmentCommand>().Execute();

            var app = container.GetRequiredService<CommandLineApplication>();
            return await app.ExecuteAsync(args);
        }
        catch (Exception exception)
        {
            var logger = loggerFactory.CreateLogger(typeof(Program));
            logger.LogCritical(exception, "Admin CLI encountered an error");
            return 1;
        }
        finally
        {
            loggerFactory.Dispose();
        }
    }
}
