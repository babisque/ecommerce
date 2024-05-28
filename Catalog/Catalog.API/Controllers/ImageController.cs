using Catalog.Core.DTO.Image;
using Catalog.Core.Entities;
using Catalog.Core.Logging;
using Catalog.Core.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.API.Controllers;

[ApiController]
[Route("/[controller]")]
public class ImageController : ControllerBase
{
    private readonly IImageRepository _imageRepository;
    private readonly ILogger<ImageController> _logger;

    public ImageController(IImageRepository imageRepository, ILogger<ImageController> logger)
    {
        _imageRepository = imageRepository;
        _logger = logger;
    }

    [HttpPost]
    public async Task<ActionResult<Image>> Post([FromForm] ImagePostReq req)
    {
        _logger.LogInformation("POST request received to create a new image");
        try
        {
            var image = new Image
            {
                Picture = req.Image,
                ProductId = req.ProductId,
                CreatedAt = DateTime.Now
            };
            
            await _imageRepository.CreateAsync(image);
            _logger.LogInformation($"Image created successfully with ID {image.Id}.");
            return CreatedAtAction(nameof(Get), new { id = image.Id }, req);
        }
        catch (Exception e)
        {
            CustomLogger.LogFile = true;
            _logger.LogError(e, "Error occurred while creating a new product.");
            return BadRequest(new { Errormessage = e.Message });
        }
    }

    [HttpGet("{productId:int}")]
    public async Task<ActionResult<Image>> Get([FromRoute] int productId)
    {
        _logger.LogInformation($"GET request received to retrieve image for product ID {productId}.");
        try
        {
            var imageBinary = _imageRepository.GetImageByProductIdAsync(productId).Result.Picture;
            if (imageBinary is null)
                return NoContent();
            
            _logger.LogInformation($"Image retrieved successfully for product ID {productId}.");
            return File(imageBinary, "image/png");
        }
        catch (Exception e)
        {
            CustomLogger.LogFile = true;
            _logger.LogError(e, "Error occurred while retrieving images");
            return NotFound(new { ErrorMessage = e.Message });
        }
    }
}