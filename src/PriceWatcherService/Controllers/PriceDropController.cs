using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PriceWatcher.Models;
using PriceWatcher.Repositories;

namespace PriceWatcher.Controllers;

[ApiController]
[Route("pricedrops")]
public class PriceDropController : ControllerBase
{
    private readonly IPriceWatcherRepository _repository;
    private readonly ILogger<PriceDropController> _logger;

    public PriceDropController(IPriceWatcherRepository repository, ILogger<PriceDropController> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    [HttpPost("invoke")]
    [Authorize(Policy = "RequiresAdminScope")]
    public IActionResult InvokePriceDrop([FromBody] PriceDropModel model)
    {
        var dropped = _repository.DropPrice(model.ProductId, model.DropPriceBy);
        if (!dropped)
        {
            return NotFound($"Product with Id {model.ProductId} not found");
        }

        return Ok(new ResponseMessage
        {
            Message = $"Dropped price for {model.ProductId} by {model.DropPriceBy * 100}%."
        });
    }
}
