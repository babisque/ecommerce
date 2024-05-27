using Catalog.Core.Entities;
using Catalog.Core.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Infrastructure.Repositories;

public class EntityRepository<T> : IRepository<T> where T : EntityBase
{
    private readonly ApplicationDbContext _context;
    protected DbSet<T> DbSet;

    public EntityRepository(ApplicationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        DbSet = _context.Set<T>();
    }

    public async Task<IList<T>> GetAllAsync()
    {
        try
        {
            return await DbSet.ToListAsync();
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }

    public async Task<T> GetByIdAsync(int id)
        => await DbSet.FirstOrDefaultAsync(e => e.Id == id) ?? throw new Exception("Entity not found");

    public async Task CreateAsync(T entity)
    {
        try
        {
            DbSet.Add(entity);
            await _context.SaveChangesAsync();
        }
        catch (Exception e)
        {
            throw new Exception("Error creating entity", e);
        }
    }

    public async Task UpdateAsync(T entity)
    {
        try
        {
            DbSet.Update(entity);
            await _context.SaveChangesAsync();
        }
        catch (Exception e)
        {
            throw new Exception("Error updating entity", e);
        }
    }

    public async Task RemoveAsync(int id)
    {
        try
        {
            DbSet.Remove(await GetByIdAsync(id));
            await _context.SaveChangesAsync();
        }
        catch (Exception e)
        {
            throw new Exception("Error removing entity", e);
        }
    }
}