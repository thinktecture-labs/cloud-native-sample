using Dapr.Client;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using OrdersService.Configuration;
using OrdersService.Entities;
using OrdersService.Models;
using Swashbuckle.AspNetCore.Annotations;

namespace OrdersService.Controllers;

[ApiController]
[Produces("application/json")]
[Route("orders")]
public class OrdersController : ControllerBase
{

    private readonly DaprClient _dapr;
    private readonly OrdersServiceConfiguration _config;
    private readonly ILogger<OrdersController> _logger;

    public OrdersController(DaprClient dapr, OrdersServiceConfiguration config, ILogger<OrdersController> logger)
    {
        _dapr = dapr;
        _config = config;
        _logger = logger;
    }


    [HttpPost]
    [Route("", Name = "CreateOrder")]
    [SwaggerOperation(OperationId = "CreateOrder", Tags=new []{"Orders"}, Summary = "Create a new order", Description = "Invoke this endpoint to place a new order")]
    [SwaggerResponse(202, Description = "Order has been accepted")]
    [SwaggerResponse(400)]
    [SwaggerResponse(500)]
    public async Task<IActionResult> CreateOrderAsync([FromBody]CreateOrderModel model)
    {

        var id = Guid.NewGuid();
        var now = DateTime.Now;
        
        _logger.LogTrace("Order ({Id}) submitted at {Now} by {CustomerName}", id, now.ToShortTimeString(), model.CustomerName);

        var newOrder = model.ToEntity(id, now);

        await _dapr.PublishEventAsync(_config.CreateOrderPubSubName, _config.CreateOrderTopicName, newOrder, CancellationToken.None)!;

        return Accepted(new { OrderId = newOrder.Id });
    }

    [HttpGet]
    [Route("{id:guid}", Name = "GetOrderById")]
    [SwaggerOperation(OperationId = "GetOrderById", Tags=new []{"Orders"}, Summary = "Load an order", Description = "This endpoint tries to load an order by its id")]
    [SwaggerResponse(200, Description = "The order", Type = typeof(OrderDetailsModel))]
    [SwaggerResponse(400)]
    [SwaggerResponse(404, Description = "No order with given id was found")]
    [SwaggerResponse(500)]
    public async Task<IActionResult> GetOrderByIdAsync([FromRoute] Guid id)
    {
        //todo!: load order from store
        Order found = null;
        
        if (found == null)
        {
            _logger.LogTrace("Order with id {Id} not found. Will result in 404", id);
            return NotFound();
        }

        var detailsModel = found.ToDetailsModel();
        return Ok(detailsModel);
    }
}
