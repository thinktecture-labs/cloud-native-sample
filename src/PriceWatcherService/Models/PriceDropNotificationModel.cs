namespace PriceWatcher.Models
{
    public class PriceDropNotificationModel
    {
        public string Recipient { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public double Price { get; set; }
    }
}
