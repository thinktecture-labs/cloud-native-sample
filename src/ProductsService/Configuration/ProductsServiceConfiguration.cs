using Microsoft.Extensions.Logging.Console;

namespace ProductsService.Configuration;

public class ProductsServiceConfiguration
{
    public const string SectionName = "ProductsService";
 
    public ProductsServiceConfiguration()
    {
        IdentityServer = new IdentityServerConfiguration();
        Authorization = new Authorization();
    }
    public string ConsoleFormatterName { get; set; } = ConsoleFormatterNames.Json;
    public bool DisableConsoleLog { get; set; }
    public string ZipkinEndpoint { get; set; }
    public bool ExposePrometheusMetrics {get;set;}
    public string ApplicationInsightsConnectionString {get;set;}

    public IdentityServerConfiguration IdentityServer { get;set; }
    public Authorization Authorization { get; set; }
    public string ConnectionString { get; set; }
}
