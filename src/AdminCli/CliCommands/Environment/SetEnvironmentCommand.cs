using System;
using AdminCli.Configuration;
using McMaster.Extensions.CommandLineUtils;

namespace AdminCli.CliCommands.Environment;

public sealed class SetEnvironmentCommand : ICliCommand
{
    public SetEnvironmentCommand(IConfigurationManager configurationManager) =>
        ConfigurationManager = configurationManager;

    private IConfigurationManager ConfigurationManager { get; }

    public void ConfigureCommand(CommandLineApplication config)
    {
        config.Description = "Sets the target environment to one of the following values: \"local\" or \"cloud\"";

        var targetArgument = config.Argument("target",
                                             "The environment you want to target. Can be either \"local\" or \"cloud\".")
                                   .IsRequired();
        
        config.OnExecute(() =>
        {
            var target = targetArgument.Value;

            string gatewayUrl;
            string identityServerUrl;
            if (string.Equals("cloud", target, StringComparison.OrdinalIgnoreCase))
            {
                gatewayUrl = AppSettings.DefaultCloudGatewayUrl;
                identityServerUrl = IdentityServerSettings.DefaultCloudServerUrl;
            }
            else if (string.Equals("local", target, StringComparison.OrdinalIgnoreCase))
            {
                gatewayUrl = AppSettings.DefaultLocalGatewayUrl;
                identityServerUrl = IdentityServerSettings.DefaultLocalServerUrl;
            }
            else
            {
                Console.WriteLine($"\"{target}\" is no valid argument");
                return;
            }
            
            var appSettings = ConfigurationManager.CurrentSettings;
            if (gatewayUrl.Equals(appSettings.GatewayUrl) &&
                identityServerUrl.Equals(appSettings.IdentityServerSettings.ServerUrl))
            {
                Console.WriteLine("No change required");
                return;
            }
                
            appSettings.IdentityServerSettings.ServerUrl = identityServerUrl;
            appSettings.GatewayUrl = gatewayUrl;
            ConfigurationManager.StoreAppSettingsSecurely(appSettings);
            Console.Write("Changed settings to target: ");
            Console.Write(gatewayUrl);
            Console.Write(" and ");
            Console.WriteLine(identityServerUrl);
        });
    }
}
