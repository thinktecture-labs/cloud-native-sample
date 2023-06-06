using System.Text.Json.Serialization;

namespace PriceDropNotifier.Models;

public class NotificationRequest
{
    [JsonPropertyName("recipient")]
    public string Recipient { get; set; } = string.Empty;

    [JsonPropertyName("productName")]
    public string ProductName { get; set; } = string.Empty;
    
    [JsonPropertyName("price")]
    public double Price { get; set; }
}
