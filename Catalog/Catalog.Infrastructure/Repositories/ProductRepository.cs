using Catalog.Core.Entities;
using Catalog.Core.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Infrastructure.Repositories;

public class ProductRepository(ApplicationDbContext context) : EntityRepository<Product>(context), IProductRepository
{
    private readonly ApplicationDbContext _context = context;

    public async Task<Product> GetProductByIdAsync(int id)
    {
        var product = await _context.Products
                          .Include(p => p.Images)
                          .Include(p => p.Categories)
                          .FirstOrDefaultAsync(p => p.Id == id)
                      ?? throw new Exception("This product doesn't exist");

        return product;
    }
}