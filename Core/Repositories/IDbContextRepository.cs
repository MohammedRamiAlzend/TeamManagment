namespace Core.Repositories;
public interface IDbContextRepository<T> where T : class
{
    Task<T> GetByIdAsync(int id);
    Task<IQueryable<T>> GetAllAsync();
    Task AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task RemoveAsync(T entity);
}
