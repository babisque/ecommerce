using Catalog.Core.DTO;
using Catalog.Core.DTO.Product;
using Catalog.Core.Entities;
using Catalog.Core.Logging;
using Catalog.Core.Repositories;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.API.Controllers
{
    [ApiController]
    [Route("/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IProductRepository _productRepository;
        private readonly ILogger<ProductController> _logger;
        private readonly IValidator<Product> _validator;

        public ProductController(IProductRepository productRepository, ILogger<ProductController> logger, IValidator<Product> validator)
        {
            _productRepository = productRepository;
            _logger = logger;
            _validator = validator;
        }

        [HttpPost]
        public async Task<ActionResult<Product>> Post([FromBody] ProductPostReq req)
        {
            _logger.LogInformation("POST request received to create a new product.");
            
            try
            {
                var product = new Product
                {
                    Name = req.Name,
                    Description = req.Description,
                    Price = req.Price,
                    Categories = req.Category,
                    Stock = req.Stock
                };
                
                ValidationResult result = await _validator.ValidateAsync(product);
                if (!result.IsValid)
                {
                    
                    var errors = result.Errors
                        .GroupBy(e => e.PropertyName)
                        .ToDictionary(
                            g => g.Key,
                            g => g.Select(e => e.ErrorMessage).ToArray()
                        );
                    
                    return BadRequest(new ValidationProblemDetails(errors));
                }

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

                var res = new List<ProductGetRes>();
                foreach (var product in products)
                {
                    res.Add(new ProductGetRes
                    {
                        Id = product.Id,
                        Name = product.Name,
                        Description = product.Description,
                        Price = product.Price,
                        Categories = product.Categories,
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

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ProductGetRes>> GetProduct([FromRoute] int id)
        {
            _logger.LogInformation($"GET request received to retrieve product with ID {id}.");
            try
            {
                var product = await _productRepository.GetProductByIdAsync(id);
                if (product == null)
                {
                    _logger.LogWarning($"Product with ID {id} not found.");
                    return NotFound();
                }

                var res = new ProductGetRes
                {
                    Id = product.Id,
                    Name = product.Name,
                    Description = product.Description,
                    Price = product.Price,
                    Categories = product.Categories,
                    ImagesId = product.Images.Select(i => i.Id).ToList(),
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
        public async Task<ActionResult> Put([FromRoute] int id, [FromBody] ProductUpdateReq req)
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
                product.Categories = req.Categories ?? product.Categories;
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
    }
}
