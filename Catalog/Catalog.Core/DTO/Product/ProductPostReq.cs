namespace Catalog.Core.DTO.Product;

public class ProductPostReq
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public List<int> Categories { get; set; }
    public int Stock { get; set; }
}