using Catalog.Core.Entities;

namespace Catalog.Core.DTO.Product;

public class ProductGetRes
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public List<Entities.Category> Categories { get; set; } = new List<Entities.Category>();
    public List<int> ImagesId { get; set; } = new List<int>();
    public int Stock { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}