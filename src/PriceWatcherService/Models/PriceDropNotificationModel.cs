namespace PriceWatcher.Models
{
    public class PriceDropNotificationModel
    {
        public string Recipient { get; set; }
        public string ProductName { get; set; }
        public double Price { get; set; }
    }
}
