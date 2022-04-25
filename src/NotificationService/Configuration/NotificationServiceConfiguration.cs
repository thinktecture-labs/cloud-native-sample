namespace NotificationService.Configuration;

public class NotificationServiceConfiguration
{
    public const string SectionName = "NotificationService";

    public string OnOrderProcessedMethodName { get; set; } = "onOrderProcessed";
}
