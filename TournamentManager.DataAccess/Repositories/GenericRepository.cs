using Microsoft.EntityFrameworkCore;
using TournamentManager.Core.Interfaces.Repositories;

namespace TournamentManager.DataAccess.Repositories;

internal class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
{
    private readonly DbSet<TEntity> _dbset;

    internal GenericRepository(DbSet<TEntity> dbset)
    {
        _dbset = dbset;
    }

    public virtual async Task AddAsync(TEntity entity)
    {
        await _dbset.AddAsync(entity);
    }

    public virtual async Task<IEnumerable<TEntity>> GetAsync(Predicate<TEntity>? filter = null)
    {
        IEnumerable<TEntity> entities = await _dbset.ToListAsync();

        if (filter != null)
        {
            entities = entities.Where(e => filter(e));
        }

        return entities;
    }

    public virtual async Task<TEntity?> GetByIdAsync(int id)
    {
        return await _dbset.FindAsync(id);
    }

    public virtual void Update(TEntity entity)
    {
        _dbset.Update(entity);
    }
}
