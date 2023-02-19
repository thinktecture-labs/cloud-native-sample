using System.Net;
using Dapr;
using Dapr.Client;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PriceDropNotifier.Models;

namespace PriceDropNotifier.Controllers;

[ApiController]
[Route("pricedrops")]
public class PriceDropNotifierController : ControllerBase
{

    private readonly DaprClient _client;
    private readonly ILogger<PriceDropNotifierController> _logger;

    public PriceDropNotifierController(DaprClient client, ILogger<PriceDropNotifierController> logger)
    {
        _client = client;
        _logger = logger;
    }

    [HttpPost]
    [Route("notify")]
    [AllowAnonymous]
    public async Task<IActionResult> Notify([FromBody] CloudEvent<NotificationRequest> model)
    {
        // Validate Dapr API token
        // Send Notification
       
        return Ok();
    }
}
