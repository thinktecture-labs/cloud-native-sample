using Dapr;
using Dapr.Client;
using Microsoft.AspNetCore.Mvc;
using PriceDropNotifier.Models;

namespace PriceDropNotifier.Controllers;

[ApiController]
[Route("pricedropnotifier")]
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
    public async Task<IActionResult> Notify([FromBody] CloudEvent<NotificationRequest> model)
    {
        var mailBody = @$"<h1>PriceDrop for {model.Data.ProductName}</h1>
            <p>Price dropped to {model.Data.Price}</p>";

        await _client.InvokeBindingAsync("sendgrid", "create", mailBody, new Dictionary<string, string> {
            {"emailTo", model.Data.Recipient},
            {"emailFrom", "christian.weyer@thinktecture.com"},
            {"subject", $"Price drop for {model.Data.ProductName}"}
        });

        return Ok();
    }
}
