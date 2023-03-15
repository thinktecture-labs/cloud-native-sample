using System;
using Microsoft.Extensions.Configuration;
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
        var configuration = new ConfigurationBuilder().AddCommandLine(args)
                                                      .Build();

        var logLevelText = configuration["log-level"];
        return Enum.TryParse<LogLevel>(logLevelText, true, out var parsedLogLevel) ?
            parsedLogLevel :
            LogLevel.Warning;
    }
}
