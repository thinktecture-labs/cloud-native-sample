namespace Microsoft.AspNetCore.Http;

public static class HttpRequestExtensions
{
    
    const string DaprApiTokenHeaderName = "dapr-api-token";
    const string DaprApiTokenEnvironmentVariableName = "DAPR_API_TOKEN";
    public static bool HasValidDaprApiToken(this HttpRequest request)
    {
        if (request.Headers.TryGetValue(DaprApiTokenHeaderName, out var tokens))
        {
            var expected  = GetDaprApiToken();
            var actual = tokens.FirstOrDefault();
            return expected == actual;
        }
        return false;
    }

    private static string GetDaprApiToken()
    {
        var token = Environment.GetEnvironmentVariable(DaprApiTokenEnvironmentVariableName);
        if (string.IsNullOrEmpty(token))
        {
            throw new InvalidOperationException($"{DaprApiTokenEnvironmentVariableName} environment variable is not set");
        }
        return token;
    }
}
