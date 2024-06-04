using Catalog.Core.Entities;
using Microsoft.AspNetCore.Http;

namespace Catalog.Core.DTO.Product;

public class ProductPostReq
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public List<Category> Category { get; set; } = new List<Category>();
    public int Stock { get; set; }
}