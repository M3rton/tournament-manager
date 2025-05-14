namespace TournamentManager.Core.Interfaces.Repositories;

public interface IGenericRepository<TEntity> where TEntity : class
{
    Task AddAsync(TEntity entity);
    void Update(TEntity entity);
    Task<IEnumerable<TEntity>> GetAsync(Predicate<TEntity>? filter = null);
    Task<TEntity?> GetByIdAsync(int id);
}
