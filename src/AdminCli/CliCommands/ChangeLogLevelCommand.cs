using System;
using AdminCli.Configuration;
using McMaster.Extensions.CommandLineUtils;
using Serilog.Events;

namespace AdminCli.CliCommands;

public sealed class ChangeLogLevelCommand : ICliCommand
{
    public ChangeLogLevelCommand(IConfigurationManager configurationManager) =>
        ConfigurationManager = configurationManager;

    private IConfigurationManager ConfigurationManager { get; }

    public void ConfigureCommand(CommandLineApplication config)
    {
        var logLevelArgument =
            config.Argument("logLevel",
                            "Possible values are \"Verbose\", \"Debug\", \"Information\", \"Warning\", \"Error\", or \"Fatal\"")
                  .IsRequired();

        config.OnExecute(() =>
        {
            if (!Enum.TryParse<LogEventLevel>(logLevelArgument.Value, true, out var parsedLevel))
            {
                Console.WriteLine($"Could not parse \"{logLevelArgument.Value}\" to a valid log level.");
                return;
            }

            var appSettings = ConfigurationManager.CurrentSettings;
            appSettings.LogLevel = parsedLevel;
            ConfigurationManager.StoreAppSettingsSecurely(appSettings);
            Console.WriteLine($"Log level was set to \"{parsedLevel.ToString()}\".");
        });
    }
}
