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
        private readonly ICategoryRepository _categoryRepository;
        private readonly ILogger<ProductController> _logger;
        private readonly IValidator<ProductPostReq> _validator;

        public ProductController(IProductRepository productRepository, ILogger<ProductController> logger, IValidator<ProductPostReq> validator, ICategoryRepository categoryRepository)
        {
            _productRepository = productRepository;
            _logger = logger;
            _validator = validator;
            _categoryRepository = categoryRepository;
        }

        [HttpPost]
        public async Task<ActionResult<Product>> Post([FromBody] ProductPostReq req)
        {
            try
            {
                ValidationResult result = await _validator.ValidateAsync(req);
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
                
                List<Category> categories = await FetchCategoriesAsync(req.Categories);
                
                var product = new Product
                {
                    Name = req.Name,
                    Description = req.Description,
                    Categories = categories,
                    Price = req.Price,
                    Stock = req.Stock
                };

                await _productRepository.CreateAsync(product);
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
            try
            {
                var products = await _productRepository.GetAllProductsAsync();
                if (products.Count == 0)
                    return NotFound();

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

                return Ok(res);
            }
            catch (Exception e)
            {
                CustomLogger.LogFile = true;
                _logger.LogError(e, "Error occurred while retrieving products.");
                return BadRequest(new { ErrorMessage = e.Message });
            }
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ProductGetRes>> GetProduct([FromRoute] int id)
        {
            try
            {
                var product = await _productRepository.GetProductByIdAsync(id);
                if (product.Id == 0)
                    return NotFound();

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

                return Ok(res);
            }
            catch (Exception e)
            {
                CustomLogger.LogFile = true;
                _logger.LogError(e, $"Error occurred while retrieving product ID {id}.");
                return BadRequest(new { ErrorMessage = e.Message });
            }
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put([FromRoute] int id, [FromBody] ProductUpdateReq req)
        {
            try
            {
                var product = await _productRepository.GetByIdAsync(id);
                if (product.Id == 0)
                    return NotFound();

                product.Name = req.Name ?? product.Name;
                product.Description = req.Description ?? product.Description;
                product.Price = req.Price ?? product.Price;
                product.Categories = req.Categories ?? product.Categories;
                product.Stock = req.Stock ?? product.Stock;
                product.UpdatedAt = DateTime.Now;

                await _productRepository.UpdateAsync(product);
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
            try
            {
                var product = await _productRepository.GetByIdAsync(id);
                if (product.Id == 0)
                    return NotFound();

                await _productRepository.RemoveAsync(product.Id);
                return Ok();
            }
            catch (Exception e)
            {
                CustomLogger.LogFile = true;
                _logger.LogError(e, $"Error occurred while deleting product with ID {id}.");
                return BadRequest(new { ErrorMessage = e.Message });
            }
        }
        
        private async Task<List<Category>> FetchCategoriesAsync(IEnumerable<int> categoryIds)
        {
            var categories = new List<Category>();
            foreach (var categoryId in categoryIds)
            {
                var category = await _categoryRepository.GetByIdAsync(categoryId);
                if (category.Id > 0)
                    categories.Add(category);
            }
            return categories;
        }
    }
}
