using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using AdminCli.Configuration;
using IdentityModel.Client;

namespace AdminCli.HttpAccess;

public sealed class HttpIdentityService
{
    public HttpIdentityService(IHttpClientFactory httpClientFactory,
                               JwtSecurityTokenHandler jwtSecurityTokenHandler)
    {
        HttpClientFactory = httpClientFactory;
        JwtSecurityTokenHandler = jwtSecurityTokenHandler;
    }

    private IHttpClientFactory HttpClientFactory { get; }
    private JwtSecurityTokenHandler JwtSecurityTokenHandler { get; }
    private DiscoveryDocumentResponse? DiscoveryDocument { get; set; }

    public async Task<TokenInfo> GetTokenInfoAsync(IdentityServerSettings settings,
                                                   CancellationToken cancellationToken = default)
    {
        using var httpClient = HttpClientFactory.CreateClient();
        DiscoveryDocument ??= await httpClient.GetDiscoveryDocumentAsync(settings.ServerUrl, cancellationToken);

        var response = await httpClient.RequestClientCredentialsTokenAsync(
            new ()
            {
                Address = DiscoveryDocument.TokenEndpoint,
                ClientId = settings.ClientId,
                ClientSecret = settings.ClientSecret,
                Scope = settings.Scopes
            },
            cancellationToken
        );

        if (response.IsError)
            throw new IOException("Could not obtain Bearer token", response.Exception);

        var token = JwtSecurityTokenHandler.ReadJwtToken(response.AccessToken);
        return new (response.AccessToken, token.ValidTo);
    }
}
