using McMaster.Extensions.CommandLineUtils;

namespace AdminCli.CliCommands;

public interface ICliCommand
{
    void ConfigureCommand(CommandLineApplication config);
}
