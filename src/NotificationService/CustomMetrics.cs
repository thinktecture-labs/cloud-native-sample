using System.Diagnostics.Metrics;
using System.Reflection;

namespace NotificationService;

public class CustomMetrics
{
    public static readonly Meter Default = new Meter("NotificationService",
       Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "0.0.0");

    public static readonly Counter<long> NotificationsSent =
        Default.CreateCounter<long>("cloud_native_sample_notifications_sent",
            description: "Number of notifications sent via SignalR");
}
