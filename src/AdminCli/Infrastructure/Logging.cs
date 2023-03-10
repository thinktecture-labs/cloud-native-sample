using Serilog;

namespace AdminCli.Infrastructure;

public static class Logging
{
    public static ILogger CreateLogger()
    {
        var logger = new LoggerConfiguration().WriteTo.Console()
                                              .CreateLogger();
        Log.Logger = logger;
        return logger;
    }
}
