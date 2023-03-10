using AdminCli.Configuration;
using McMaster.Extensions.CommandLineUtils;

namespace AdminCli.CliCommands;

public sealed class TargetCloudCommand : ICliCommand
{
    public TargetCloudCommand(IConfigurationManager configurationManager)
    {
        ConfigurationManager = configurationManager;
    }

    private IConfigurationManager ConfigurationManager { get; }

    public void ConfigureCommand(CommandLineApplication config)
    {
        config.Description = "Configures the admin CLI to target the Thinktecture cloud gateway and Identity server.";
        config.OnExecute(() =>
        {
            ConfigurationManager.ConfigureTargetAndStoreSettings(IdentityServerSettings.DefaultCloudServerUrl,
                                                                 AppSettings.DefaultCloudGatewayUrl);
        });
    }
}
