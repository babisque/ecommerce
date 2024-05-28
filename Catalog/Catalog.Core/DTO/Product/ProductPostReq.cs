using Microsoft.AspNetCore.Http;

namespace Catalog.Core.DTO.Product;

public class ProductPostReq
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public string? Category { get; set; }
    public int Stock { get; set; }
}