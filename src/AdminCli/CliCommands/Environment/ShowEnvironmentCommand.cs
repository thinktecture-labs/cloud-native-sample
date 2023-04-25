using System;
using AdminCli.Configuration;
using McMaster.Extensions.CommandLineUtils;

namespace AdminCli.CliCommands.Environment;

public sealed class ShowEnvironmentCommand : ICliCommand
{
    public ShowEnvironmentCommand(IConfigurationManager configurationManager) =>
        ConfigurationManager = configurationManager;

    private IConfigurationManager ConfigurationManager { get; }

    public void ConfigureCommand(CommandLineApplication config)
    {
        config.Description = "Outputs the gateway and identity server URLs you are currently targeting";
        config.OnExecute(Execute);
    }

    public void Execute()
    {
        var appSettings = ConfigurationManager.CurrentSettings;
        Console.Write("You are targeting gateway ");
        Console.Write(appSettings.GatewayUrl);
        Console.Write(" and identity server ");
        Console.WriteLine(appSettings.IdentityServerSettings.ServerUrl);
    }
}
