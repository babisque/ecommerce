using Microsoft.AspNetCore.Http;

namespace Catalog.Core.DTO.Image;

public class ImagePostReq
{
    public IFormFile? Image { get; set; }
    public int ProductId { get; set; }
}