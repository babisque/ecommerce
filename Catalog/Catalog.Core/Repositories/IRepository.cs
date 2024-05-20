using Catalog.Core.Entities;

namespace Catalog.Core.Repositories;

public interface IRepository<T> where T : EntityBase
{
    IList<T> GetAll();
    T GetById(int id);
    void Create(T entity);
    void Update(T entity);
    void Remove(int id);
}