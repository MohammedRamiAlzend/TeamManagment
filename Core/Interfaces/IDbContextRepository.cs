using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace TMS.Core.Interfaces;
public interface IDbContextRepository<T> where T : class
{
    Task<DbRequest<T>> GetAsync(Expression<Func<T, bool>>? filter = null,
        Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null);
    Task<DbRequest<List<T>>> GetAllAsync(Expression<Func<T, bool>>? filter = null,
        Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderby = null);
    Task<DbRequest> AddAsync(T entity);
    Task<DbRequest> UpdateAsync(T entity);
    Task<DbRequest> RemoveAsync(T entity);
}
