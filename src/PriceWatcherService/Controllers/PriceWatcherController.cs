using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Mvc;
using PriceWatcher.Models;
using PriceWatcher.Repositories;

namespace PriceWatcher.Controllers;

[ApiController]
[Route("pricewatcher")]
public class PriceWatcherController : ControllerBase
{
    private IPriceWatcherRepository _repository;
    private ILogger<PriceWatcherController> _logger;
    
    public PriceWatcherController(IPriceWatcherRepository repository, ILogger<PriceWatcherController> logger)
    {
        _repository = repository;
        _logger = logger;
    }


    [HttpPost("register")]
    public IActionResult RegisterAsync([FromBody] RegisterModel model)
    {
        var success = _repository.Register(model.Email, model.ProductId, model.Price);
        if (!success)
        {
            return NotFound($"Product with Id {model.ProductId} not found");
        }
        return Ok(new ResponseMessage 
        {
            Message = "Registration succeeded"
        });
    }
    
}
