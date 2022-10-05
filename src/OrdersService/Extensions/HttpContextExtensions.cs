using System.Security.Claims;

namespace OrdersService.Extensions;

public static class HttpContextExtensions
{
    public static string GetUserName(this HttpContext c)
    {
        var nameClaim = c.User.FindFirst(ClaimTypes.Name);
        return nameClaim != null ? nameClaim.Value : "N/A";
    }

    public static string GetUserId(this HttpContext c)
    {
        var subClaim = c.User.FindFirst(ClaimTypes.NameIdentifier);
        return subClaim != null ? subClaim.Value : "N/A";
    }
}
