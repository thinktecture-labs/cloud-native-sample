using Yarp.ReverseProxy.Transforms;
using Yarp.ReverseProxy.Transforms.Builder;

namespace Gateway.TransformProviders;

public class RequestLogTransformProvider: ITransformProvider
{
    private readonly ILogger<RequestLogTransformProvider> _logger;

    public RequestLogTransformProvider(ILogger<RequestLogTransformProvider> logger)
    {
        _logger = logger;
    }
    
    public void ValidateRoute(TransformRouteValidationContext context)
    {
        
    }

    public void ValidateCluster(TransformClusterValidationContext context)
    {
        
    }

    public void Apply(TransformBuilderContext context)
    {
        context.AddRequestTransform(tc =>
        {
            _logger.LogTrace("Gateway will proxy route for {Path}", tc.Path.Value);
            return ValueTask.CompletedTask;
        });

    }
}
