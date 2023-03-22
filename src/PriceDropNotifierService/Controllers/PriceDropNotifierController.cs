using System.Net;
using Dapr;
using Dapr.Client;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PriceDropNotifier.Data.UnitOfWork;
using PriceDropNotifier.Models;

namespace PriceDropNotifier.Controllers;

[ApiController]
[Route("pricedrops")]
public class PriceDropNotifierController : ControllerBase
{
    private readonly DaprClient _client;
    private readonly ILogger<PriceDropNotifierController> _logger;
    private readonly INotificationUnitOfWork _unitOfWork;

    public PriceDropNotifierController(DaprClient client,
                                       ILogger<PriceDropNotifierController> logger,
                                       INotificationUnitOfWork unitOfWork)
    {
        _client = client;
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    [HttpPost]
    [Route("notify")]
    [AllowAnonymous]
    public async Task<IActionResult> Notify([FromBody] CloudEvent<NotificationRequest> model,
                                            CancellationToken cancellationToken)
    {
        // I think we can get rid of this try-catch block. The same thing is done
        // by the HTTP pipeline of ASP.NET Core
        try
        {
            if (!HttpContext.Request.HasValidDaprApiToken())
            {
                _logger.LogWarning("OnOrderProcessed: Received invalid Dapr API token.");
                return Unauthorized();
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while validating Dapr API token");
            return StatusCode((int) HttpStatusCode.InternalServerError);
        }

        var data = model.Data;
        _logger.LogInformation("Received notification request for {productName} with price {price}",
                               data.ProductName,
                               data.Price);
        var targetRegistrations = await _unitOfWork.GetRegistrationsToBeNotifiedAsync(data.ProductId,
                                                                                      data.Price,
                                                                                      cancellationToken);

        if (targetRegistrations.Count == 0)
        {
            _logger.LogInformation("No price watcher registrations were found for {Data}", data);
            return Ok();
        }

        var mailBody = @$"<h1>PriceDrop for {data.ProductName}</h1>
            <p>Price dropped to {data.Price:F2}</p>";
        foreach (var registration in targetRegistrations)
        {
            await _client.InvokeBindingAsync(
                "email",
                "create",
                mailBody,
                new Dictionary<string, string>
                {
                    { "emailTo", registration.Email },
                    { "subject", $"📉 Price drop for {data.ProductName}" }
                }
            );
        }

        return Ok();
    }
}
