using Catalog.Core.DTO;
using Catalog.Core.Entities;
using Catalog.Core.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.API.Controllers;

[ApiController]
[Route("/[controller]")]
public class ProductController : ControllerBase // TODO: Create the entire CRUD
{
    private readonly IProductRepository _productRepository;

    public ProductController(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromForm] ProductDto req)
    {
        try
        {
            using var ms = new MemoryStream();
            await req.Image.CopyToAsync(ms);
            
            var product = new Product
            {
                Name = req.Name,
                Description = req.Description,
                Price = req.Price,
                Category = req.Category,
                Image = ms.ToArray(),
                Stock = req.Stock
            };
            _productRepository.Create(product);
            return Created();
        }
        catch (Exception e)
        {
            return BadRequest(e);
        }
    }
}