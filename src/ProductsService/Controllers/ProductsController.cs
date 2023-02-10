using Microsoft.AspNetCore.Mvc;
using ProductsService.Data.Repositories;
using ProductsService.Extensions;
using ProductsService.Models;
using Swashbuckle.AspNetCore.Annotations;

namespace ProductsService.Controllers;

[ApiController]
[Route("products")]
[Produces("application/json")]
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
    [SwaggerOperation(OperationId = "GetAllProducts", Tags = new []{"Products"}, Summary = "Get all products", Description = "Call into this endpoint to retrieve a list of all products")]
    [SwaggerResponse(200, Description = "A list with all products", Type = typeof(IEnumerable<ProductListModel>))]
    [SwaggerResponse(500)]
    public async Task<IActionResult> GetAllProductsAsync()
    {
        var res = await _repository.GetAllAsync();

        return Ok(res.Select(p => p.ToListModel()));
    }

    [HttpPost]
    [Route("", Name = "CreateProduct")]
    [SwaggerOperation(OperationId = "CreateProduct", Tags = new []{"Products"}, Summary = "Create a new product", Description = "Call into this endpoint to create a new product")]
    [SwaggerResponse(201, Description = "The created product", Type = typeof(ProductDetailsModel))]
    [SwaggerResponse(400)]
    [SwaggerResponse(500)]
    public async Task<IActionResult> CreateProductAsync([FromBody]ProductCreateModel model)
    {
        var product = model.ToEntity();
        var created = await _repository.CreateAsync(product);

        return CreatedAtRoute("GetProductById", new { id = product.Id }, product.ToDetailsModel());
    }

    [HttpGet]
    [Route("{id:guid}", Name = "GetProductById")]
    [SwaggerOperation(OperationId = "GetProductById", Tags = new []{"Products"}, Summary = "Get a product by its id", Description = "Call into this endpoint to retrieve a particular product by its identifier")]
    [SwaggerResponse(200, Description = "The found product", Type = typeof(ProductDetailsModel))]
    [SwaggerResponse(400)]
    [SwaggerResponse(404, Description = "No product was found with the given id")]
    [SwaggerResponse(500)]
    public async Task<IActionResult> GetProductByIdAsync([FromRoute]Guid id){
        var res = await _repository.GetByIdAsync(id);

        if (res == null) 
        {
            _logger.LogTrace("Product with id {Id} not found. Will result in 404", id);

            return NotFound();
        }

        return Ok(res.ToDetailsModel());
    }
}
