namespace Catalog.Core.Entities;

public class Image : EntityBase
{
    public byte[] Picture { get; set; }
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;
}