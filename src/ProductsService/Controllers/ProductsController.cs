using Microsoft.AspNetCore.Mvc;
using ProductsService.Repositories;

namespace ProductsService.Controllers;

[ApiController]
[Route("products")]
public class ProductsController : ControllerBase
{ 

    public ProductsController(IProductsRepository repository, ILogger<ProductsController> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    private readonly ILogger<ProductsController> _logger;
    private readonly IProductsRepository _repository;

    [HttpGet]
    [Route("", Name= "GetAllProducts")]
    public async Task<IActionResult> GetAllProductsAsync()
    {
        var res = await _repository.GetAllAsync();
        return Ok(res);
    }

    [HttpGet]
    [Route("{id}", Name = "GetProductById")]
    public async Task<IActionResult> GetProductByIdAsync([FromRoute]Guid id){
        var res = await _repository.GetByIdAsync(id);
        if (res == null) 
        {
            return NotFound();
        }
        return Ok(res);
    }
}
