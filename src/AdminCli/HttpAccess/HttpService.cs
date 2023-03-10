using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using AdminCli.Configuration;
using IdentityModel.Client;

namespace AdminCli.HttpAccess;

public sealed class HttpService : IHttpService
{
    public HttpService(IHttpClientFactory httpClientFactory,
                       HttpIdentityService httpIdentityService,
                       IConfigurationManager configurationManager,
                       JsonSerializerOptions jsonSerializerOptions)
    {
        HttpClientFactory = httpClientFactory;
        HttpIdentityService = httpIdentityService;
        ConfigurationManager = configurationManager;
        JsonSerializerOptions = jsonSerializerOptions;
    }

    private IHttpClientFactory HttpClientFactory { get; }
    private HttpIdentityService HttpIdentityService { get; }
    private IConfigurationManager ConfigurationManager { get; }
    private JsonSerializerOptions JsonSerializerOptions { get; }

    public async Task<T?> GetAsync<T>(string relativeUrl, CancellationToken cancellationToken = default)
    {
        using var httpClient = await CreateHttpClientAsync(cancellationToken);
        return await httpClient.GetFromJsonAsync<T>(relativeUrl, JsonSerializerOptions, cancellationToken);
    }

    public async Task PostAsync<T>(string relativeUrl, T content, CancellationToken cancellationToken = default)
    {
        using var httpClient = await CreateHttpClientAsync(cancellationToken);
        using var response = await httpClient.PostAsJsonAsync(relativeUrl,
                                                              content,
                                                              JsonSerializerOptions,
                                                              cancellationToken);
        response.EnsureSuccessStatusCode();
    }

    private async Task<HttpClient> CreateHttpClientAsync(CancellationToken cancellationToken)
    {
        var appSettings = ConfigurationManager.CurrentSettings;
        if (!appSettings.TryGetAccessToken(out var accessToken))
        {
            appSettings.CurrentTokenInfo =
                await HttpIdentityService.GetTokenInfoAsync(appSettings.IdentityServerSettings, cancellationToken);
            ConfigurationManager.StoreAppSettingsSecurely(appSettings);
            accessToken = appSettings.CurrentTokenInfo.AccessToken;
        }

        var httpClient = HttpClientFactory.CreateClient();
        httpClient.SetBearerToken(accessToken);
        httpClient.BaseAddress = new (appSettings.GatewayUrl);
        return httpClient;
    }
}
