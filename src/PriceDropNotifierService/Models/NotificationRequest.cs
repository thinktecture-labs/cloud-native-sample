namespace PriceDropNotifier.Models;

public class NotificationRequest 
{
    public string Recipient {get;set;}
    public string ProductName {get;set;}
    public double Price {get;set;}

}
