using System.Text.Json.Serialization;

namespace PriceDropNotifier.Models;

public class NotificationRequest
{
    [JsonPropertyName("productId")]

    public Guid ProductId { get; set; }

    [JsonPropertyName("productName")]
    public string ProductName { get; set; } = string.Empty;

    [JsonPropertyName("price")]
    public double Price { get; set; }
}
