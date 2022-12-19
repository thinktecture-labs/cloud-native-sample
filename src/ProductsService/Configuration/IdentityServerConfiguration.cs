namespace ProductsService.Configuration;

public class IdentityServerConfiguration 
{
    public string Authority {get;set;}
    public string MetadataAddress {get;set;}
    public bool RequireHttpsMetadata { get; set;} = true;
}
