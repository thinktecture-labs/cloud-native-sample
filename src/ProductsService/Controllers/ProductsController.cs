using Microsoft.AspNetCore.Mvc;
using ProductsService.Repositories;

namespace ProductsService.Controllers;

[ApiController]
[Route("products")]
public class ProductsController : ControllerBase
{ 

    private readonly ILogger<ProductsController> _logger;
    private readonly IProductsRepository _repository;
    
    public ProductsController(IProductsRepository repository, ILogger<ProductsController> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    [HttpGet]
    [Route("", Name= "GetAllProducts")]
    public async Task<IActionResult> GetAllProductsAsync()
    {
        var res = await _repository.GetAllAsync();
        return Ok(res);
    }

    [HttpGet]
    [Route("{id:guid}", Name = "GetProductById")]
    public async Task<IActionResult> GetProductByIdAsync([FromRoute]Guid id){
        var res = await _repository.GetByIdAsync(id);
        if (res == null) 
        {
            _logger.LogTrace("Product with id {Id} not found. Will result in 404", id);
            return NotFound();
        }
        return Ok(res);
    }
}
