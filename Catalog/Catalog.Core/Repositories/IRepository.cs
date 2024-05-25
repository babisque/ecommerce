using Catalog.Core.Entities;

namespace Catalog.Core.Repositories;

public interface IRepository<T> where T : EntityBase
{
    Task<IList<T>> GetAllAsync();
    Task<T> GetByIdAsync(int id);
    Task CreateAsync(T entity);
    Task UpdateAsync(T entity);
    Task RemoveAsync(int id);
}