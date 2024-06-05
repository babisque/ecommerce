using System.Text.Json.Serialization;

namespace Catalog.Core.Entities;

public class Category : EntityBase
{
    public string Name { get; set; }
    [JsonIgnore]
    public List<Product> Products { get; set; }
}