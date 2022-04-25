using Gateway.Configuration;
using Yarp.ReverseProxy.Transforms;
using Yarp.ReverseProxy.Transforms.Builder;

namespace Gateway.TransformProviders {
    public class DaprTransformProvider : ITransformProvider
    {
        private readonly GatewayConfiguration _config;
        private readonly string[] _daprRoutes = { Constants.OrdersRouteName, Constants.ProductsRouteName }; 
        public DaprTransformProvider(GatewayConfiguration config)
        {
            _config = config;
        }
        public void Apply(TransformBuilderContext context)
        {
            if(_daprRoutes.Contains(context.Route.RouteId))
            {
                context.AddRequestTransform(t => {
                    Thread.Sleep(3000);
                    t.ProxyRequest.RequestUri = new Uri($"{_config.DaprEndpoint}/v1.0/invoke/products/method{t.Path.Value}");
                    return ValueTask.CompletedTask;
                });
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
