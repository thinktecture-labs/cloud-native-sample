using Microsoft.AspNetCore.Mvc;

namespace PriceDropNotifier.Controllers;

[ApiController]
[Route("pricedrops")]
public class PriceDropNotifierController : ControllerBase
{
    [HttpPost]
    [Route("notify")]
    public IActionResult Notify()
    {
        return Ok();
    }
}
