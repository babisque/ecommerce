using Microsoft.AspNetCore.Http;

namespace Catalog.Core.DTO;

public class ProductDto
{
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public string Categoria { get; set; }
    public byte[] Image { get; set; }
    public int Stock { get; set; }
}