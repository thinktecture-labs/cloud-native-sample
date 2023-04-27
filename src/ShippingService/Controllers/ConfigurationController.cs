using Microsoft.AspNetCore.Mvc;
using ShippingService.Configuration;

namespace ShippingService.Controllers;

[ApiController]
[Route("configuration")]
public class ConfigurationController : ControllerBase
{
    private readonly ShippingServiceConfiguration _configuration;

    public ConfigurationController(ShippingServiceConfiguration configuration)
    {
        _configuration = configuration;
    }

    [HttpGet]
    [Route("")]
    public IActionResult GetConfiguration()
    {
        return Ok(_configuration);
    }
}
