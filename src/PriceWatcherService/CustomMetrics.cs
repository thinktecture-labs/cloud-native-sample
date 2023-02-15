using System.Reflection;
using System.Diagnostics.Metrics;

namespace PriceWatcher;

public class CustomMetrics
{
    public static readonly Meter Default = new Meter("PriceWatcherService",
        Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "0.0.0");

    public static readonly Counter<long> PriceWatchers =
        Default.CreateCounter<long>("cloud_native_new_price_watch",
            description: "Number of users watching for a price drop");

    public static readonly Counter<long> PriceDrops =
        Default.CreateCounter<long>("cloud_native_price_drop",
            description: "Number of a price drop issued");
    
}
