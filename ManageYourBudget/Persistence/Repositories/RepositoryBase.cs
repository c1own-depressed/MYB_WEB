using Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

public abstract class RepositoryBase<T> : IRepositoryBase<T>
    where T : class
{
    protected readonly DbContext _context;

    public RepositoryBase(DbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _context.Set<T>().ToListAsync();
    }

    public async Task<T> GetByIdAsync(int id)
    {
        var entity = await _context.Set<T>().FindAsync(id);
        if (entity == null)
        {
            throw new Exception($"Entity of type {typeof(T).Name} with ID {id} not found.");
        }

        return entity;
    }

    public async Task AddAsync(T entity)
    {
        await _context.Set<T>().AddAsync(entity);
    }

    public void Update(T entity)
    {
        _context.Set<T>().Update(entity);
    }

    public void Delete(T entity)
    {
        _context.Set<T>().Remove(entity);
    }

    public async Task SaveAsync()
    {
        await _context.SaveChangesAsync();
    }
}
