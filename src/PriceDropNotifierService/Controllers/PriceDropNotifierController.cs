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
        _logger.LogInformation("Received notification request for {productName} with price {price}", model.Data.ProductName, model.Data.Price);
        var mailBody = @$"<h1>PriceDrop for {model.Data.ProductName}</h1>
            <p>Price dropped to {model.Data.Price:F2}</p>";

        await _client.InvokeBindingAsync("email", "create", mailBody, new Dictionary<string, string> {
            {"emailTo", model.Data.Recipient}, 
            {"subject", $"📉 Price drop for {model.Data.ProductName}"}
        });

        return Ok();
    }
}
