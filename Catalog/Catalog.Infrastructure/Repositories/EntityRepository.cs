using Catalog.Core.Entities;
using Catalog.Core.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Infrastructure.Repositories;

public class EntityRepository<T> : IRepository<T> where T : EntityBase
{
    private ApplicationDbContext _context;
    protected DbSet<T> DbSet;

    public EntityRepository(ApplicationDbContext context)
    {
        _context = context;
        DbSet = _context.Set<T>();
    }


    public IList<T> GetAll()
        => DbSet.ToList();

    public T GetById(int id)
        => DbSet.FirstOrDefault(e => e.Id == id) ?? throw new Exception("Entity not found");

    public void Create(T entity)
    {
        DbSet.Add(entity);
        _context.SaveChanges();
    }

    public void Update(T entity)
    {
        DbSet.Update(entity);
        _context.SaveChanges();
    }

    public void Remove(int id)
    {
        DbSet.Remove(GetById(id));
        _context.SaveChanges();
    }
}