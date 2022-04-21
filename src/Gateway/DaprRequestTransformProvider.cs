using Yarp.ReverseProxy.Transforms;
using Yarp.ReverseProxy.Transforms.Builder;

namespace Gateway {
    public class DaprRequestTransformProvider : ITransformProvider
    {
        public void Apply(TransformBuilderContext context)
        {

            if(context.Route.RouteId == "products"){
                context.AddRequestTransform(t => {
                    Thread.Sleep(3000);
                    t.ProxyRequest.RequestUri = new Uri($"http://localhost:9001/v1.0/invoke/products/method{t.Path.Value}");
                    return ValueTask.CompletedTask;
                });
            }
        }

        public void ValidateCluster(TransformClusterValidationContext context)
        {
          
        }

        public void ValidateRoute(TransformRouteValidationContext context)
        {
            
           // check if route needs transformation....
        }
    }
}
