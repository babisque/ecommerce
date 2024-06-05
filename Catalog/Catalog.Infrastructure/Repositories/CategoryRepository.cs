using Catalog.Core.Entities;
using Catalog.Core.Repositories;

namespace Catalog.Infrastructure.Repositories;

public class CategoryRepository(ApplicationDbContext context) : EntityRepository<Category>(context), ICategoryRepository;