using Dapr.Client;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using OrdersService.Configuration;
using OrdersService.Entities;
using OrdersService.Models;

namespace OrdersService.Controllers;

[ApiController]
[Route("orders")]
public class OrdersController : ControllerBase
{

    private readonly DaprClient _dapr;
    private readonly OrdersServiceConfiguration _config;
    private readonly ILogger<OrdersController> _logger;

    public OrdersController(DaprClient dapr, IOptions<OrdersServiceConfiguration> options, ILogger<OrdersController> logger)
    {
        _dapr = dapr;
        _config = options.Value ?? throw new ArgumentNullException(nameof(options));
        _logger = logger;
    }


    [HttpPost]
    [Route("", Name = "CreateOrder")]
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
