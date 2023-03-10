using AdminCli.CliCommands;
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
                Description = "The Command Line Interface of the Thinktecture Cloud Native Sample that allows admins to run internal commands."
            };
            app.HelpOption(inherited: true);

            var context = new CommandConfigurationContext(app, container);
            context.ConfigureCommand<ListProductsCommand>("list-products");

            return app;
        });
    }
}
