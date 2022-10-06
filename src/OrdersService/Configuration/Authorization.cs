namespace OrdersService.Configuration;

public class Authorization
{
    public string RequiredClaimName { get; set; } = "scope";
    public string RequiredClaimValue {get;set;} = "sample";
}
