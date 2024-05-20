namespace Catalog.Core.Entities;

public class Product : EntityBase
{
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public string Categoria { get; set; }
    public byte[]? Image { get; set; }
    public int Stock { get; set; }
}