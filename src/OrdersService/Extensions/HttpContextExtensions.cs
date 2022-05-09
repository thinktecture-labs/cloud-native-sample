namespace OrdersService.Extensions;

public static class HttpContextExtensions
{
    public static string GetUserName(this HttpContext c)
    {
        //TODO: extract from (claim) token 
        return "John Doe";
    }

    public static string GetUserId(this HttpContext c)
    {
        //TODO: extract from (claim) token
        return Guid.Empty.ToString();
    }
}
