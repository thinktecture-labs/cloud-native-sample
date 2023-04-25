using System;
using McMaster.Extensions.CommandLineUtils;

namespace AdminCli.CliCommands.Environment;

public sealed class EnvironmentCommand : ICliCommand
{
    public EnvironmentCommand(IServiceProvider container) => Container = container;

    private IServiceProvider Container { get; }

    public void ConfigureCommand(CommandLineApplication config)
    {
        config.Description = "Allows you to show or set the targeted environment";
        var context = new CommandConfigurationContext(config, Container);
        context.ConfigureCommand<ShowEnvironmentCommand>("show");
        context.ConfigureCommand<SetEnvironmentCommand>("set");
        
        config.OnExecute(() => config.ShowHelp());
    }
}
