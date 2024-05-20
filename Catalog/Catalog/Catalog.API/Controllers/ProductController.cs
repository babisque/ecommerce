using Catalog.Core.DTO;
using Catalog.Core.Entities;
using Catalog.Core.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.API.Controllers;

[ApiController]
[Route("/[controller]")]
public class ProductController : ControllerBase
{
    private readonly IProductRepository _productRepository;

    public ProductController(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] ProductDto req)
    {
        try
        {
            var product = new Product
            {
                Name = req.Name,
                Description = req.Description,
                Price = req.Price,
                Categoria = req.Categoria,
                Image = req.Image,
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