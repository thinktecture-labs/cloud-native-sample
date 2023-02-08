using System.Diagnostics.Metrics;
using System.Reflection;

namespace Gateway;

public class CustomMetrics
{

    public static readonly Meter Default = new Meter("Gateway",
        Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "0.0.0");

    public static readonly Counter<long> ProxiedRequests = Default.CreateCounter<long>("cloud_native_sample_proxied_requests",
        description: "Number of proxied requests");
}
