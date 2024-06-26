namespace Catalog.Core.DTO.Product;

public class ProductUpdateReq
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public decimal? Price { get; set; }
    public List<Entities.Category> Categories { get; set; } = [];
    public int? Stock { get; set; }
}