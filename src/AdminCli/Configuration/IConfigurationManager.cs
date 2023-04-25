namespace AdminCli.Configuration;

public interface IConfigurationManager
{
    AppSettings CurrentSettings { get; }
    void StoreAppSettingsSecurely(AppSettings appSettings);
}
