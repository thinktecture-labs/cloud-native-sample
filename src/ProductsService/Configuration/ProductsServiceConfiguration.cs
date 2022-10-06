namespace ProductsService.Configuration;

public class ProductsServiceConfiguration
{
    public const string SectionName = "ProductsService";

    public bool UseFakeImplementation { get; set; } = false;
    public string ZipkinEndpoint { get;set; }
}
