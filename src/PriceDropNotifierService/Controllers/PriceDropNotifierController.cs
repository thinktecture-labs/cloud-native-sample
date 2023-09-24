using Dapr;
using Dapr.Client;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PriceDropNotifier.Configuration;
using PriceDropNotifier.Models;

namespace PriceDropNotifier.Controllers;

[ApiController]
[Route("pricedrops")]
public class PriceDropNotifierController : ControllerBase
{
    private readonly DaprClient _client;
    private readonly PriceDropNotifierServiceConfiguration _cfg;
    private readonly ILogger<PriceDropNotifierController> _logger;

    public PriceDropNotifierController(
        DaprClient client,
        PriceDropNotifierServiceConfiguration cfg,
        ILogger<PriceDropNotifierController> logger
    )
    {
        _client = client;
        _cfg = cfg;
        _logger = logger;
    }
    
    [HttpPost]
    [Route("notify")]
    [AllowAnonymous]
    public async Task<IActionResult> Notify([FromBody] CloudEvent<NotificationRequest> @event)
    {
        if (!HttpContext.Request.HasValidDaprApiToken())
            return Unauthorized();

        _logger.LogInformation(
            "Received notification request for {ProductName} with price {Price}",
            @event.Data.ProductName,
            @event.Data.Price
        );
        
        var mailBody = @$"<h1>PriceDrop for {@event.Data.ProductName}</h1>
            <p>Price dropped to {@event.Data.Price:F2}</p>";

        await _client.InvokeBindingAsync(
            _cfg.NotificationBindingName,
            _cfg.NotificationBindingOperation,
            mailBody,
            new Dictionary<string, string>
            {
                { "emailTo", @event.Data.Recipient },
                { "subject", $"📉 Price drop for {@event.Data.ProductName}" }
            }
        );
        
        return Ok();
    }
}
