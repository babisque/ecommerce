using Catalog.Core.Entities;
using Catalog.Core.Repositories;

namespace Catalog.Infrastructure.Repositories;

public class ProductRepository(ApplicationDbContext context) : EntityRepository<Product>(context), IProductRepository;