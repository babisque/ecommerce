using Microsoft.AspNetCore.Http;

namespace Catalog.Core.Services;

public static class ImageServices
{
    public static async Task<byte[]> GetImageBytesAsync(IFormFile image)
    {
        using var ms = new MemoryStream();
        await image.CopyToAsync(ms);
        return ms.ToArray();
    }
    
    public static bool IsImageValid(byte[] imageBytes)
    {
        Dictionary<string, byte[]> imgTypes = new Dictionary<string, byte[]>
        {
            { "JPG", new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 } },
            { "JPEG", new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 } },
            { "PNG", new byte[] { 0x89, 0x50, 0x4E, 0x47 } }
        };

        foreach (var imgType in imgTypes)
        {
            if (imageBytes.Length >= imgType.Value.Length &&
                imgType.Value.SequenceEqual(imageBytes.Take(imgType.Value.Length)))
            {
                return true;
            }
        }
        return false;
    }
}