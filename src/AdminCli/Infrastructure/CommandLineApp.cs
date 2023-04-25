using AdminCli.CliCommands;
using AdminCli.CliCommands.Environment;
using AdminCli.CliCommands.Products;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.DependencyInjection;

namespace AdminCli.Infrastructure;

public static class CommandLineApp
{
    public static IServiceCollection AddCommandLineApp(this IServiceCollection services)
    {
        return services.AddSingleton(container =>
        {
            var app = new CommandLineApplication
            {
                Name = "admin-cli",
                Description = "The Command Line Interface of the Thinktecture Cloud Native Sample that allows admins to run internal commands.",
                // UnrecognizedArgumentHandling = UnrecognizedArgumentHandling.CollectAndContinue
            };
            app.HelpOption(inherited: true);

            app.Option("-l|--log-level",
                       "Allows you to set the the log level for the current run. The default log level is \"Warning\".",
                       CommandOptionType.SingleValue)
               .Inherited = true;

            var context = new CommandConfigurationContext(app, container);
            context.ConfigureCommand<ProductsCommand>("products")
                   .ConfigureCommand<EnvironmentCommand>("environment");

            app.OnExecute(() => app.ShowHelp());

            return app;
        });
    }
}
