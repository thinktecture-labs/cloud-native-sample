using System.Text.Json;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.DependencyInjection;

namespace AdminCli.Configuration;

public static class ConfigurationModule
{
    public static IServiceCollection AddConfiguration(this IServiceCollection services) =>
        services.AddSingleton<IConfigurationManager>(container =>
                 {
                     var dataProtector = container.GetRequiredService<IDataProtector>();
                     var jsonSerializerOptions = container.GetRequiredService<JsonSerializerOptions>();
                     return SecureFileConfigurationManager.Create(dataProtector, jsonSerializerOptions);
                 })
                .AddSingleton<IDataProtector>(
                     container =>
                         container.GetRequiredService<IDataProtectionProvider>()
                                  .CreateProtector("admin-cli")
                 )
                .AddDataProtection()
                .Services;
}
