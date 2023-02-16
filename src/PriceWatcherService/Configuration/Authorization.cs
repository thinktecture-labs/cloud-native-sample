namespace PriceWatcher.Configuration;

public class Authorization
{
    public string RequiredClaimName { get; set; } = "scope";
    public string RequiredClaimValue {get;set;} = "sample";
    public string AdminScopeName {get;set;} = "admin";
}
