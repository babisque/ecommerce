namespace Catalog.Core.Entities;

public class Product : EntityBase
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public string? Category { get; set; }
    public ICollection<ImageData> Images { get; set; } = new List<ImageData>();
    public int Stock { get; set; }
}