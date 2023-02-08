using Dapr;
using Dapr.Client;
using Microsoft.AspNetCore.Mvc;
using OrdersService.Configuration;
using OrdersService.Data.Entities;
using OrdersService.Data.Repositories;
using OrdersService.Extensions;
using OrdersService.Models;
using Swashbuckle.AspNetCore.Annotations;

namespace OrdersService.Controllers;

[ApiController]
[Produces("application/json")]
[Route("orders")]
public class OrdersController : ControllerBase
{
    private readonly IOrdersRepository _repository;
    private readonly DaprClient _dapr;
    private readonly OrdersServiceConfiguration _config;
    private readonly ILogger<OrdersController> _logger;

    public OrdersController(IOrdersRepository repository, DaprClient dapr, OrdersServiceConfiguration config,
        ILogger<OrdersController> logger)
    {
        _repository = repository;
        _dapr = dapr;
        _config = config;
        _logger = logger;
    }

    [HttpPost]
    [Route("", Name = "CreateOrder")]
    [SwaggerOperation(OperationId = "CreateOrder", Tags = new[] { "Orders" }, Summary = "Create a new order",
        Description = "Invoke this endpoint to place a new order")]
    [SwaggerResponse(202, Description = "Order has been accepted")]
    [SwaggerResponse(400)]
    [SwaggerResponse(401)]
    [SwaggerResponse(500)]
    public async Task<IActionResult> CreateOrderAsync([FromBody] CreateOrderModel model)
    {
        var id = Guid.NewGuid();
        var now = DateTime.Now;
        var userName = HttpContext.GetUserName();

        _logger.LogTrace("Order ({id}) submitted at {now} by {userName}", id, now.ToShortTimeString(), userName);

        var newOrder = model.ToEntity(id, now, HttpContext.GetUserId(), userName);

        await _repository.AddNewOrderAsync(newOrder);

        var traceIdentifier = HttpContext.TraceIdentifier;
        var cloudEvent = new CloudEvent<Order>(newOrder){
            Type = "com.thinktecture/new-order"
        };
        var metadata = new Dictionary<string, string>();
        metadata.Add("traceparent", traceIdentifier);

        await _dapr.PublishEventAsync<CloudEvent<Order>>(
            _config.CreateOrderPubSubName, 
            _config.CreateOrderTopicName,
            cloudEvent,
            metadata,
            CancellationToken.None)!;
        CustomMetrics.OrdersCreated.Add(1);
        
        return Accepted(new { OrderId = newOrder.Id });
    }

    [HttpGet]
    [Route("", Name = "GetOrders")]
    [SwaggerOperation(OperationId = "GetOrders", Tags = new[] { "Orders" }, Summary = "Load all orders",
        Description = "This endpoint returns all orders")]
    [SwaggerResponse(200, Description = "The order", Type = typeof(IEnumerable<OrderListModel>))]
    [SwaggerResponse(400)]
    [SwaggerResponse(401)]
    [SwaggerResponse(500)]
    public async Task<IActionResult> GetOrdersAsync()
    {
        var found = await _repository.GetAllOrdersAsync();

        return Ok(found.Select(f => f.ToListModel()));
    }
}
