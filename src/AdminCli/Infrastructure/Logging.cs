using System;
using AdminCli.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Core;
using Serilog.Events;

namespace AdminCli.Infrastructure;

public static class Logging
{
    private static LoggingLevelSwitch LoggingLevelSwitch { get; } = new (LogEventLevel.Warning);
    
    public static ILogger CreateLogger()
    {
        var logger = new LoggerConfiguration().MinimumLevel.ControlledBy(LoggingLevelSwitch)
                                              .WriteTo.Console()
                                              .CreateLogger();
        Log.Logger = logger;
        return logger;
    }

    public static void SetLogLevelFromAppSettings(this IServiceProvider container)
    {
        var configurationManager = container.GetRequiredService<IConfigurationManager>();
        LoggingLevelSwitch.MinimumLevel = configurationManager.CurrentSettings.LogLevel;
    }
}
