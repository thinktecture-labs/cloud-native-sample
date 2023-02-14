using AuthenticationService.Configuration;
using AuthenticationService.Extensions;

try
{
    var builder = WebApplication.CreateBuilder(args);

    var cfg = new IdentityServerConfig();
    var cfgSection = builder.Configuration.GetSection(IdentityServerConfig.SectionName);

    if (cfgSection == null || !cfgSection.Exists())
    {
        throw new ApplicationException(
            $"Could not find Authentication configuration. Please ensure a '{IdentityServerConfig.SectionName}' exists");
    }

    cfgSection.Bind(cfg);
    builder.Services.AddSingleton(cfg);

    // logging
    builder.ConfigureLogging(cfg);
    // traces
    builder.ConfigureTracing(cfg);
    // metrics
    builder.ConfigureMetrics(cfg);

    var app = builder
        .ConfigureServices()
        .ConfigurePipeline();

    app.Run();
}
catch (Exception ex)
{
    Console.WriteLine("Unhandled exception" + ex);
}
finally
{
    Console.WriteLine("Shut down complete");
}
