using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using NotificationService.Configuration;
using NotificationService.Models;
using Dapr;
using Microsoft.AspNetCore.Authorization;
using System.Net;

namespace NotificationService.Controllers;

[ApiController]
[Route("orders")]
public class OrdersController : Controller
{
    private readonly IHubContext<NotificationHub> _hubContext;
    private readonly NotificationServiceConfiguration _config;
    private readonly ILogger<OrdersController> _logger;

    public OrdersController(IHubContext<NotificationHub> hubContext, NotificationServiceConfiguration config, ILogger<OrdersController> logger)
    {
        _hubContext = hubContext;
        _config = config;
        _logger = logger;
    }

    [HttpPost]
    [Route("processed")]
    [Topic("orders", "processed_orders")]
    [AllowAnonymous]
    public async Task<IActionResult> OnOrderProcessedAsync([FromBody] DispatchedOrder order)
    {
        try
        {
            if (!HttpContext.Request.HasValidDaprApiToken())
            {
                _logger.LogWarning("OnOrderProcessed: Received invalid Dapr API token.");
                return Unauthorized();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while validating Dapr API token.");
            return StatusCode((int)HttpStatusCode.InternalServerError);
        }

        if (order == null)
        {
            _logger.LogWarning("OnOrderProcessed: Received null as order.");
            return StatusCode(500);
        }
        _logger.LogTrace("OnOrderProcessed: Received order with {OrderId} and {UserId}", order.OrderId, order.UserId);

        var group = _hubContext.Clients.Group(order.UserId);
        if (group == null)
        {
            _logger.LogWarning("SignalR group with name {Name} not found, request will fail with 404", order.UserId);

            return NotFound();
        }

        await group.SendAsync(_config.OnOrderProcessedMethodName, order.OrderId);
        CustomMetrics.NotificationsSent.Add(1);
        return Ok();
    }
}
