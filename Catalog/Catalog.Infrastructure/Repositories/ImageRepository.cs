using Catalog.Core.Entities;
using Catalog.Core.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Infrastructure.Repositories;

public class ImageRepository(ApplicationDbContext context) : EntityRepository<ImageData>(context), IImageRepository;