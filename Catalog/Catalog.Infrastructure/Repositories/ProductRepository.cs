using Catalog.Core.Entities;
using Catalog.Core.Repositories;

namespace Catalog.Infrastructure.Repositories;

public class ProductRepository(ApplicationDbContext context) : EntityRepository<Product>(context), IProductRepository
{
    public byte[]? GetImage(int id)
    {
        var product = GetByIdAsync(id);
        return product.Result.Image;
    }
}