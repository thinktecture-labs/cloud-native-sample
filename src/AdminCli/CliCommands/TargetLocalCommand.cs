using AdminCli.Configuration;
using McMaster.Extensions.CommandLineUtils;

namespace AdminCli.CliCommands;

public sealed class TargetLocalCommand : ICliCommand
{
    public TargetLocalCommand(IConfigurationManager configurationManager) =>
        ConfigurationManager = configurationManager;

    private IConfigurationManager ConfigurationManager { get; }

    public void ConfigureCommand(CommandLineApplication config)
    {
        config.Description =
            "Configures the admin CLI to target your cluster that runs on localhost (via docker compose up -d).";
        config.OnExecute(() =>
        {
            ConfigurationManager.ConfigureTargetAndStoreSettings(IdentityServerSettings.DefaultLocalServerUrl,
                                                                 AppSettings.DefaultLocalGatewayUrl);
        });
    }
}
