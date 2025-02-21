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
    Task<DbRequest<PaginatedDbRequest<T>>> GetAllPaginatedAsync(
        Expression<Func<T, bool>>? filter = null,
        Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        int pageNumber = 1,
        int pageSize = 10);
    Task<DbRequest> AddAsync(T entity);
    Task<DbRequest> UpdateAsync(T entity);
    Task<DbRequest> RemoveAsync(Expression<Func<T, bool>>? filter = null);
}
