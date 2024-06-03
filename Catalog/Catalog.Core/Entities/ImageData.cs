namespace Catalog.Core.Entities;

public class ImageData : EntityBase
{
    public byte[]? ImageBytes { get; set; }
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;
}