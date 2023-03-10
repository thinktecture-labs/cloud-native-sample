using System;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.DependencyInjection;

namespace AdminCli.CliCommands;

public readonly record struct CommandConfigurationContext(CommandLineApplication Config,
                                                          IServiceProvider Container)
{
    public CommandConfigurationContext ConfigureCommand<TCommand>(string commandName)
        where TCommand : class, ICliCommand
    {

        var command = Container.GetRequiredService<TCommand>();
        Config.Command(commandName,
                       subConfig => command.ConfigureCommand(subConfig));
        return this;
    }
}
