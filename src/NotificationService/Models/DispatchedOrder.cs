using System.Text.Json.Serialization;

namespace NotificationService.Models;

public class DispatchedOrder
{
    [JsonPropertyName("orderId")]
    public string OrderId { get; set; }

    [JsonPropertyName("userId")]
    public string UserId { get; set; }
    [JsonPropertyName("userName")]
    public string UserName { get; set; }
}

