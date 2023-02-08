using Gateway.Configuration;
using Yarp.ReverseProxy.Transforms;
using Yarp.ReverseProxy.Transforms.Builder;

namespace Gateway.TransformProviders
{
    public class DaprTransformProvider : ITransformProvider
    {
        private readonly GatewayConfiguration _config;
        private readonly ILogger<DaprTransformProvider> _logger;
        private readonly int _daprHttpPort;

        public DaprTransformProvider(GatewayConfiguration config, IConfiguration configuration,
            ILogger<DaprTransformProvider> logger)
        {
            _config = config;
            _daprHttpPort = configuration.GetValue<int>("DAPR_HTTP_PORT");
            _logger = logger;
        }

        public void Apply(TransformBuilderContext context)
        {
            if (_daprHttpPort != 0)
            {
                if (_config.DaprServiceNames.Contains(context.Route.RouteId))
                {
                    _logger.LogTrace("Daprizing route with id {RouteId}", context.Route.RouteId);

                    CustomMetrics
                        .ProxiedRequests
                        .Add(1, new KeyValuePair<string, object?>("kind", "dapr"));
                    context.AddRequestTransform(t =>
                    {
                        t.ProxyRequest.RequestUri =
                            new Uri(
                                $"http://localhost:{_daprHttpPort}/v1.0/invoke/{context.Route.RouteId}/method{t.Path.Value}");

                        return ValueTask.CompletedTask;
                    });
                }
            }
        }

        public void ValidateCluster(TransformClusterValidationContext context)
        {
        }

        public void ValidateRoute(TransformRouteValidationContext context)
        {
        }
    }
}
