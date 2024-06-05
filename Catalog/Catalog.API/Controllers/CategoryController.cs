using Catalog.Core.DTO.Category;
using Catalog.Core.Entities;
using Catalog.Core.Logging;
using Catalog.Core.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.API.Controllers;

[ApiController]
[Route("/[controller]")]
public class CategoryController : ControllerBase
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly ILogger<CategoryController> _logger;

    public CategoryController(ICategoryRepository categoryRepository, ILogger<CategoryController> logger)
    {
        _categoryRepository = categoryRepository;
        _logger = logger;
    }

    [HttpPost]
    public async Task<ActionResult> Post([FromBody] CategoryPostReq req)
    {
        try
        {
            var category = new Category
            {
                Name = req.Name,
                CreatedAt = DateTime.Now
            };

            await _categoryRepository.CreateAsync(category);
            return CreatedAtAction(nameof(GetCategoryById), new { categoryId = category.Id }, category);
        }
        catch (Exception e)
        {
            CustomLogger.LogFile = true;
            _logger.LogError(e, "Error occurred while creating a new category");
            return BadRequest(new { ErrorMessage = e.Message });
        }
    }

    [HttpGet]
    public async Task<ActionResult> Get()
    {
        try
        {
            var categories = await _categoryRepository.GetAllAsync();
            if (categories.Count == 0)
                return NoContent();

            var res = new List<CategoryGetRes>();
            foreach (var category in categories)
            {
                res.Add(new CategoryGetRes
                {
                    Id = category.Id,
                    Name = category.Name
                });
            }

            return Ok(res);
        }
        catch (Exception e)
        {
            CustomLogger.LogFile = true;
            _logger.LogError(e, "Error occurred while retrieving categories.");
            return BadRequest(new { ErrorMessage = e.Message });
        }
    }

    [HttpGet("{categoryId:int}")]
    public async Task<ActionResult> GetCategoryById([FromRoute] int categoryId)
    {
        try
        {
            var category = await _categoryRepository.GetByIdAsync(categoryId);
            if (String.IsNullOrEmpty(category.Name))
                return NotFound();

            var res = new CategoryGetRes
            {
                Id = category.Id,
                Name = category.Name
            };

            return Ok(res);
        }
        catch (Exception e)
        {
            CustomLogger.LogFile = true;
            _logger.LogError(e, "Error occurred while retrieving a new category");
            return BadRequest(new { ErrorMessage = e.Message });
        }
    }

    [HttpPut("{categoryId:int}")]
    public async Task<ActionResult> Update([FromRoute] int categoryId, [FromBody] CategoryUpdateReq req)
    {
        try
        {
            var category = await _categoryRepository.GetByIdAsync(categoryId);
            if (category.Id == 0)
                return NotFound();

            category.Name = req.Name;
            category.UpdatedAt = DateTime.Now;

            await _categoryRepository.UpdateAsync(category);
            return Ok();
        }
        catch (Exception e)
        {
            CustomLogger.LogFile = true;
            _logger.LogError(e, "Error occurred while updating a new category");
            return BadRequest(new { ErrorMessage = e.Message });
        }
    }

    [HttpDelete("{categoryId:int}")]
    public async Task<ActionResult> Delete([FromRoute] int categoryId)
    {
        try
        {
            var category = await _categoryRepository.GetByIdAsync(categoryId);
            if (category.Id == 0)
                return NotFound();

            await _categoryRepository.RemoveAsync(category.Id);
            return Ok();
        }
        catch (Exception e)
        {
            CustomLogger.LogFile = true;
            _logger.LogError(e, $"Error occurred while deleting category with ID {categoryId}.");
            return BadRequest(new { ErrorMessage = e.Message });
        }
    }
}