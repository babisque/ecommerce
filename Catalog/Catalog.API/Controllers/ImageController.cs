using Catalog.Core.DTO.Image;
using Catalog.Core.Entities;
using Catalog.Core.Logging;
using Catalog.Core.Repositories;
using Microsoft.AspNetCore.Mvc;
using static Catalog.Core.Services.ImageServices;

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
    public async Task<ActionResult> Post([FromForm] ImagePostReq req)
    {
        try
        {
            if (req.Image != null && !IsImageValid(await GetImageBytesAsync(req.Image)))
                return BadRequest("Image type not supported");
            
            var image = new ImageData
            {
                ImageBytes = req.Image is not null ? await GetImageBytesAsync(req.Image) : null,
                ProductId = req.ProductId,
                CreatedAt = DateTime.Now
            };
            
            await _imageRepository.CreateAsync(image);
            return CreatedAtAction(nameof(GetImageById), new { imageId = image.Id }, image);
        }
        catch (Exception e)
        {
            CustomLogger.LogFile = true;
            _logger.LogError(e, "Error occurred while creating a new product.");
            return BadRequest(new { Errormessage = e.Message });
        }
    }

    [HttpGet("{imageId:int}")]
    public async Task<ActionResult> GetImageById([FromRoute] int imageId)
    {
        try
        {
            var image = await _imageRepository.GetByIdAsync(imageId);
            if (image.ImageBytes == null)
                return NotFound($"Image not found for ID {imageId}.");
            
            return File(image.ImageBytes, "image/png");
        }
        catch (Exception e)
        {
            CustomLogger.LogFile = true;
            _logger.LogError(e, $"Error occurred while retrieving image ID {imageId}");
            return StatusCode(500, new { ErrorMessage = e.Message });
        }
    }

    [HttpDelete("{imageId:int}")]
    public async Task<ActionResult> Delete([FromRoute] int imageId)
    {
        try
        {
            var image = await _imageRepository.GetByIdAsync(imageId);
            if (image.ImageBytes == null)
                return NotFound($"Image not found for ID {imageId}.");

            await _imageRepository.RemoveAsync(image.Id);
            return Ok();
        }
        catch (Exception e)
        {
            CustomLogger.LogFile = true;
            _logger.LogError(e, $"Error ocurred while deleting image ID {imageId}");
            return BadRequest(new { ErrorMessage = e.Message });
        }
    }
}