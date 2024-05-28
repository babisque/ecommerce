using Catalog.Core.Entities;

namespace Catalog.Core.Repositories;

public interface IImageRepository : IRepository<Image>
{
    public Task<Image> GetImageByProductIdAsync(int id);
}