using System.Text.Json.Serialization;

namespace PriceDropNotifier.Models;

public class NotificationRequest 
{

    [JsonPropertyName("recipient")]
    public string Recipient {get;set;}
    [JsonPropertyName("productName")]
    public string ProductName {get;set;}
    [JsonPropertyName("price")]
    public double Price {get;set;}

}
