namespace OrdersService;

public static class HttpContextExtensions
{
    public static string GetUserName(this HttpContext c)
    {
        //todo: extract from (claim) token 
        return "John Doe";
    }

    public static string GetUserId(this HttpContext c)
    {
        //todo: extract from (claim) token
        return Guid.Empty.ToString();
    }
}
