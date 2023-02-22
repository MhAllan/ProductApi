using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using ProductService.DomainModels;
using ProductService.Models;
using ProductService.Models.Requests;
using ProductService.Repositories;

namespace ProductService.Controllers;

[Route("product")]
public class ProductController : ControllerBase // Not a fan of ApiController as it requires complex conventions
{
    readonly ProductControllerConfig _config;
    readonly IProductRepository _repository;

    public ProductController(IOptions<ProductControllerConfig> options, IProductRepository repository)
    {
        _repository= repository;
        _config = options.Value;
    }

    [HttpPost]
    [ProducesResponseType(typeof(Product), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateProduct([FromBody] CreateProductRequest request, 
        CancellationToken ct = default)
    {
        var product = request.ToProduct();
        await _repository.AddProduct(product, ct);

        return Created($"/product/{product.Id}", product);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(Product), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetProduct([FromRoute] string id, CancellationToken ct = default)
    {
        if (string.IsNullOrEmpty(id))
        {
            return BadRequest("id is required");
        }

        var prouct = await _repository.GetProduct(id, ct);
        if (prouct == null)
        {
            return NotFound($"Could not find product with id {id}");
        }

        return Ok(prouct);
    }

    [HttpGet("list/{page?}")]
    [ProducesResponseType(typeof(Product), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult GetProducts([FromRoute] ListProductsRequest request, CancellationToken ct = default)
    {
        var page = request.Page;
        var count = _config.ListPageSize;
        var skip = page.HasValue ? Math.Max(0, count * (page.Value - 1)) : 0;

        var products = _repository.GetProducts(skip, count, ct);
        
        return Ok(products);
    }
}
