using System;
using AdminCli.Configuration;

namespace AdminCli.CliCommands;

public static class Extensions
{
    public static void ConfigureTargetAndStoreSettings(this IConfigurationManager configurationManager,
                                                       string identityServerUrl,
                                                       string gatewayUrl)
    {
        var appSettings = configurationManager.CurrentSettings;
        appSettings.IdentityServerSettings.ServerUrl = identityServerUrl;
        appSettings.GatewayUrl = gatewayUrl;
        configurationManager.StoreAppSettingsSecurely(appSettings);
        Console.Write("Changed settings to target ");
        Console.WriteLine(gatewayUrl);
    }
}
