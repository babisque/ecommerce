using Catalog.Core.DTO;
using Catalog.Core.Entities;
using Catalog.Core.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.API.Controllers;

    [ApiController]
    [Route("/[controller]")]
    public class ProductController : ControllerBase
    {
        
        // TODO: Logging
        // TODO: Validations - maybe using FluentValidation
        private readonly IProductRepository _productRepository;

        public ProductController(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        [HttpPost]
        public async Task<ActionResult<Product>> Post([FromForm] ProductDtoRequest req)
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
                await _productRepository.CreateAsync(product);
                return CreatedAtAction(nameof(Post), new { product.Id }, product);
            }
            catch (Exception e)
            {
                return BadRequest(new { ErrorMessage = e.Message });
            }
        }

        [HttpGet]
        public IActionResult Get()
        {
            IList<ProductDtoResponse> res = new List<ProductDtoResponse>();
            var products = _productRepository.GetAllAsync();
            if (products?.Result == null)
                return NotFound();

            foreach (var product in products.Result)
            {
                res.Add(new ProductDtoResponse
                {
                    Id = product.Id,
                    Name = product.Name,
                    Description = product.Description,
                    Price = product.Price,
                    Category = product.Category,
                    Stock = product.Stock,
                    CreatedAt = product.CreatedAt,
                    UpdatedAt = product.UpdatedAt
                });
            }

            return Ok(res);
        }

        [HttpGet("GetImage/{productId:int}")]
        public IActionResult GetImage([FromRoute]int productId)
        {
            var imageBinary = _productRepository.GetImage(productId);
            if (imageBinary is not null)
                return File(imageBinary, "image/png");

            return NoContent();
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ProductDtoResponse>> GetProduct([FromRoute] int id)
        {
            try
            {
                var res = new ProductDtoResponse();
                var product = await _productRepository.GetByIdAsync(id);

                res.Id = product.Id;
                res.Name = product.Name;
                res.Description = product.Description;
                res.Price = product.Price;
                res.Category = product.Category;
                res.Stock = product.Stock;
                res.CreatedAt = product.CreatedAt;
                res.UpdatedAt = product.UpdatedAt;
                
                return Ok(res);
            }
            catch (Exception e)
            {
                return NotFound(new { ErrorMessage = e.Message });
            }
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put([FromRoute] int id, [FromBody] ProductDtoUpdate req)
        {
            try
            {
                var product = await _productRepository.GetByIdAsync(id);

                if (req.Name != null) product.Name = req.Name;
                if (req.Description != null) product.Description = req.Description;
                if (req.Price != null)product.Price = (decimal)req.Price;
                if (req.Category != null) product.Category = req.Category;
                if (req.Stock != null) product.Stock = (int)req.Stock;
                product.UpdatedAt = DateTime.Now;

                await _productRepository.UpdateAsync(product);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(new { ErrorMessage = e.Message });
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete([FromRoute] int id)
        {
            try
            {
                var product = await _productRepository.GetByIdAsync(id);
                await _productRepository.RemoveAsync(product.Id);
                return Ok();
            }
            catch (Exception e)
            {
                return NotFound(new { ErrorMessage = e.Message });
            }
        }
    }