using Catalog.Core.Entities;
using Catalog.Core.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Infrastructure.Repositories;

public class ImageRepository(ApplicationDbContext context) : EntityRepository<Image>(context), IImageRepository
{
    private readonly ApplicationDbContext _context = context;

    public async Task<Image> GetImageByProductIdAsync(int id)
        => await DbSet.FirstOrDefaultAsync(i => i.ProductId == id) ??
           throw new Exception("Image for this product not found");
}