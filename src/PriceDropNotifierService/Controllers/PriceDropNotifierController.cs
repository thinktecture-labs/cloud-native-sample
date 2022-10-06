using Microsoft.AspNetCore.Mvc;

namespace PriceDropNotifier.Controllers;

[ApiController]
[Route("pricedropnotifier")]
public class PriceDropNotifierController : ControllerBase
{

    private readonly ILogger<PriceDropNotifierController> _logger;

    public PriceDropNotifierController(ILogger<PriceDropNotifierController> logger)
    {
        _logger = logger;
    }
}
