using System.IO;
using System.Text.Json;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Configuration;

namespace AdminCli.Configuration;

public sealed class SecureFileConfigurationManager : IConfigurationManager
{
    public SecureFileConfigurationManager(AppSettings currentSettings,
                                          IDataProtector dataProtector,
                                          JsonSerializerOptions jsonSerializerOptions)
    {
        CurrentSettings = currentSettings;
        DataProtector = dataProtector;
        JsonSerializerOptions = jsonSerializerOptions;
    }

    private static string SettingsFilePath { get; } =
        Path.Combine(
            Path.GetDirectoryName(typeof(SecureFileConfigurationManager).Assembly.Location)!,
            "scrambledSettings.txt"
        );

    public AppSettings CurrentSettings { get; private set; }
    private IDataProtector DataProtector { get; }
    private JsonSerializerOptions JsonSerializerOptions { get; }

    public void StoreAppSettingsSecurely(AppSettings newAppSettings)
    {
        CurrentSettings = newAppSettings;
        var json = JsonSerializer.Serialize(newAppSettings, JsonSerializerOptions);
        var encryptedJson = DataProtector.Protect(json);
        File.WriteAllText(SettingsFilePath, encryptedJson);
    }

    public static SecureFileConfigurationManager Create(IDataProtector dataProtector,
                                                        JsonSerializerOptions jsonSerializerOptions)
    {
        var appSettings = DeserializeOrCreateAppSettings(dataProtector, jsonSerializerOptions);
        return new (appSettings, dataProtector, jsonSerializerOptions);
    }

    private static AppSettings DeserializeOrCreateAppSettings(IDataProtector dataProtector,
                                                              JsonSerializerOptions jsonSerializerOptions)
    {
        if (File.Exists(SettingsFilePath))
        {
            var fileContent = File.ReadAllText(SettingsFilePath);
            var json = dataProtector.Unprotect(fileContent);
            return JsonSerializer.Deserialize<AppSettings>(json, jsonSerializerOptions)!;
        }

        var appSettings = new AppSettings();
        var configuration = new ConfigurationBuilder().AddEnvironmentVariables("AdminCli_")
                                                      .Build();
        configuration.Bind(appSettings);
        return appSettings;
    }
}
