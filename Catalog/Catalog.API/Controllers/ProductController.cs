using Catalog.Core.DTO;
using Catalog.Core.Entities;
using Catalog.Core.Logging;
using Catalog.Core.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.API.Controllers
{
    [ApiController]
    [Route("/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IProductRepository _productRepository;
        private readonly ILogger<ProductController> _logger;

        public ProductController(IProductRepository productRepository, ILogger<ProductController> logger)
        {
            _productRepository = productRepository;
            _logger = logger;
        }

        [HttpPost]
        public async Task<ActionResult<Product>> Post([FromForm] ProductDtoRequest req)
        {
            _logger.LogInformation("POST request received to create a new product.");
            
            try
            {
                var product = new Product
                {
                    Name = req.Name,
                    Description = req.Description,
                    Price = req.Price,
                    Category = req.Category,
                    Image = req.Image != null ? await GetImageBytesAsync(req.Image) : null,
                    Stock = req.Stock
                };

                await _productRepository.CreateAsync(product);
                _logger.LogInformation($"Product created successfully with ID {product.Id}.");
                return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
            }
            catch (Exception e)
            {
                CustomLogger.LogFile = true;
                _logger.LogError(e, "Error occurred while creating a new product.");
                return BadRequest(new { ErrorMessage = e.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            _logger.LogInformation("GET request received to retrieve all products.");
            try
            {
                var products = await _productRepository.GetAllAsync();
                if (products == null || products.Count == 0)
                {
                    _logger.LogWarning("No products found.");
                    return NotFound();
                }

                var res = new List<ProductDtoResponse>();
                foreach (var product in products)
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

                _logger.LogInformation("Products retrieved successfully.");
                return Ok(res);
            }
            catch (Exception e)
            {
                CustomLogger.LogFile = true;
                _logger.LogError(e, "Error occurred while retrieving products.");
                return StatusCode(500, new { ErrorMessage = e.Message });
            }
        }

        [HttpGet("GetImage/{productId:int}")]
        public async Task<IActionResult> GetImage([FromRoute] int productId)
        {
            _logger.LogInformation($"GET request received to retrieve image for product ID {productId}.");

            try
            {
                var imageBinary = _productRepository.GetImage(productId);
                if (imageBinary != null)
                {
                    _logger.LogInformation($"Image retrieved successfully for product ID {productId}.");
                    return File(imageBinary, "image/png");
                }

                _logger.LogWarning($"No image found for product ID {productId}.");
                return NoContent();
            }
            catch (Exception e)
            {
                CustomLogger.LogFile = true;
                _logger.LogError(e, $"Error occurred while retrieving image for product ID {productId}.");
                return StatusCode(500, new { ErrorMessage = e.Message });
            }
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ProductDtoResponse>> GetProduct([FromRoute] int id)
        {
            _logger.LogInformation($"GET request received to retrieve product with ID {id}.");
            try
            {
                var product = await _productRepository.GetByIdAsync(id);
                if (product == null)
                {
                    _logger.LogWarning($"Product with ID {id} not found.");
                    return NotFound();
                }

                var res = new ProductDtoResponse
                {
                    Id = product.Id,
                    Name = product.Name,
                    Description = product.Description,
                    Price = product.Price,
                    Category = product.Category,
                    Stock = product.Stock,
                    CreatedAt = product.CreatedAt,
                    UpdatedAt = product.UpdatedAt
                };

                _logger.LogInformation($"Product with ID {id} retrieved successfully.");
                return Ok(res);
            }
            catch (Exception e)
            {
                CustomLogger.LogFile = true;
                _logger.LogError(e, $"Error occurred while retrieving product ID {id}.");
                return StatusCode(500, new { ErrorMessage = e.Message });
            }
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put([FromRoute] int id, [FromBody] ProductDtoUpdate req)
        {
            _logger.LogInformation($"PUT request received to update product with ID {id}.");

            try
            {
                var product = await _productRepository.GetByIdAsync(id);
                if (product == null)
                {
                    _logger.LogWarning($"Product with ID {id} not found.");
                    return NotFound();
                }

                product.Name = req.Name ?? product.Name;
                product.Description = req.Description ?? product.Description;
                product.Price = req.Price ?? product.Price;
                product.Category = req.Category ?? product.Category;
                product.Stock = req.Stock ?? product.Stock;
                product.UpdatedAt = DateTime.Now;

                await _productRepository.UpdateAsync(product);
                _logger.LogInformation($"Product with ID {id} updated successfully.");
                return Ok();
            }
            catch (Exception e)
            {
                CustomLogger.LogFile = true;
                _logger.LogError(e, $"Error occurred while updating product with ID {id}.");
                return BadRequest(new { ErrorMessage = e.Message });
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete([FromRoute] int id)
        {
            _logger.LogInformation($"DELETE request received to delete product with ID {id}.");

            try
            {
                var product = await _productRepository.GetByIdAsync(id);
                if (product == null)
                {
                    _logger.LogWarning($"Product with ID {id} not found.");
                    return NotFound();
                }

                await _productRepository.RemoveAsync(product.Id);
                _logger.LogInformation($"Product with ID {id} deleted successfully.");
                return Ok();
            }
            catch (Exception e)
            {
                CustomLogger.LogFile = true;
                _logger.LogError(e, $"Error occurred while deleting product with ID {id}.");
                return StatusCode(500, new { ErrorMessage = e.Message });
            }
        }

        private async Task<byte[]> GetImageBytesAsync(IFormFile image)
        {
            using var ms = new MemoryStream();
            await image.CopyToAsync(ms);
            return ms.ToArray();
        }
    }
}
