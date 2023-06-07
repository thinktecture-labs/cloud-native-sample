using System;
using McMaster.Extensions.CommandLineUtils;

namespace AdminCli.CliCommands.Products;

public sealed class ProductsCommand : ICliCommand
{
    public ProductsCommand(IServiceProvider serviceProvider) => ServiceProvider = serviceProvider;

    private IServiceProvider ServiceProvider { get; }

    public void ConfigureCommand(CommandLineApplication config)
    {
        config.Description = "Allows you to list products or issue a price drop";
        var context = new CommandConfigurationContext(config, ServiceProvider);
        context.ConfigureCommand<ListProductsCommand>("list");
        context.ConfigureCommand<SignUpForPriceDropCommand>("sign-up");
        context.ConfigureCommand<DropPriceCommand>("drop-price");

        config.OnExecute(() => config.ShowHelp());
    }
}
