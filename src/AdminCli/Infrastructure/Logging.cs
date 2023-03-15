using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AdminCli.Infrastructure;

public static class Logging
{
    public static ILoggerFactory CreateLoggingFactory(string[] args)
    {
        var logLevel = DetermineLogLevel(args);
        var loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddConsole()
                   .SetMinimumLevel(logLevel);
        });

        return loggerFactory;
    }

    public static IServiceCollection AddLogging(this IServiceCollection services,
                                                ILoggerFactory loggerFactory)
    {
        services.AddSingleton(loggerFactory);
        services.AddSingleton(typeof(ILogger<>), typeof(Logger<>));
        return services;
    }

    private static LogLevel DetermineLogLevel(string[] args)
    {
        var indexOfLogLevel = FindIndexForLogLevel(args);
        var logLevelArgument = indexOfLogLevel != -1 ? args[indexOfLogLevel] : "";
        var logLevel = LogLevel.Warning;
        if (Enum.TryParse<LogLevel>(logLevelArgument, true, out var parsedLogLevel))
            logLevel = parsedLogLevel;

        return logLevel;
    }

    private static int FindIndexForLogLevel(string[] args)
    {
        for (var i = 0; i < args.Length; i++)
        {
            var argument = args[i];
            if (argument is "-l" or "--log-level")
            {
                var targetIndex = i + 1;
                return targetIndex < args.Length ? targetIndex : -1;
            }
        }

        return -1;
    }
}
