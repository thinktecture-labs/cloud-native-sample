using System.Reflection;
using System.Diagnostics.Metrics;

namespace OrdersService;

public class CustomMetrics
{
    public static readonly Meter Default = new Meter("OrdersService",
        Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "0.0.0");

    public static readonly Counter<long> OrdersCreated =
        Default.CreateCounter<long>("cloud_native_sample_orders_created",
            description: "Number of orders created");
    
}
