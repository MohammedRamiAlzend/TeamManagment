namespace Core.Repositories;
public class DbContextRepository<T> : IDbContextRepository<T> where T : class
{
    private readonly DbSet<T> _dbSet;
    public DbContextRepository(DbSet<T> dbSet)
    {
        _dbSet = dbSet;
    }
    public async Task AddAsync(T entity)
    {
        throw new NotImplementedException();
    }
    public async Task<IQueryable<T>> GetAllAsync()
    {
        throw new NotImplementedException();
    }
    public async Task<T> GetByIdAsync(int id)
    {
        throw new NotImplementedException();
    }
    public async Task RemoveAsync(T entity)
    {
        throw new NotImplementedException();
    }
    public async Task UpdateAsync(T entity)
    {
        _dbSet.Attach(entity);
        _dbSet.Entry(entity).State = EntityState.Modified;
    }
}
