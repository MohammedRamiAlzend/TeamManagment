using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;

namespace TMS.Infrastructure.Repositories;

public class DbContextRepository<T>(DbSet<T> dbSet) : IDbContextRepository<T>
    where T : Entity
{
    /// <summary>
    /// Adds a new entity to the database asynchronously.
    /// </summary>
    public async Task<DbRequest> AddAsync(T entity)
    {
        if (entity == null)
        {
            return DbRequest.Failure("Entity cannot be null");
        }

        return await ExecuteOperationAsync(
            async () =>
            {
                await dbSet.AddAsync(entity);
                return DbRequest.Success();
            },
            $"Entity of type {typeof(T).Name} has been added successfully",
            $"Something went wrong while adding entity of type {typeof(T).Name}");
    }

    /// <summary>
    /// Retrieves all entities from the database asynchronously, with optional filtering, including, and ordering.
    /// </summary>
    public async Task<DbRequest<List<T>>> GetAllAsync(
        Expression<Func<T, bool>>? filter = null,
        Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null)
    {
        IQueryable<T> query = dbSet;

        try
        {
            if (filter != null) query = query.Where(filter);
            if (include != null) query = include(query);
            if (orderBy != null) query = orderBy(query);

            var result = await query.ToListAsync();
            if (result.Count == 0)
            {
                return DbRequest<List<T>>.Failure($"No entities of type {typeof(T).Name} found.");
            }

            return DbRequest<List<T>>.Success(result, "Entities retrieved successfully.");
        }
        catch (Exception e)
        {
            return DbRequest<List<T>>.Failure(
                $"Something went wrong while retrieving entities of type {typeof(T).Name}. \n Exception Message: {e.Message}");
        }
    }

    /// <summary>
    /// Retrieves all entities from the database asynchronously with pagination, filtering, including, and ordering.
    /// </summary>
    public async Task<DbRequest<PaginatedDbRequest<T>>> GetAllPaginatedAsync(
        Expression<Func<T, bool>>? filter = null,
        Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        int pageNumber = 1,
        int pageSize = 10)
    {
        IQueryable<T> query = dbSet;

        try
        {
            if (filter != null) query = query.Where(filter);
            if (include != null) query = include(query);
            if (orderBy != null) query = orderBy(query);

            var totalCount = await query.CountAsync();
            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            if (items.Count == 0)
            {
                return DbRequest<PaginatedDbRequest<T>>.Failure($"No entities of type {typeof(T).Name} found.");
            }

            var paginatedResult = new PaginatedDbRequest<T>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            return DbRequest<PaginatedDbRequest<T>>.Success(paginatedResult, "Entities retrieved successfully.");
        }
        catch (Exception e)
        {
            return DbRequest<PaginatedDbRequest<T>>.Failure(
                $"Something went wrong while retrieving paginated entities of type {typeof(T).Name}. \n Exception Message: {e.Message}");
        }
    }

    /// <summary>
    /// Retrieves a single entity from the database asynchronously, with optional filtering and including related entities.
    /// </summary>
    public async Task<DbRequest<T>> GetAsync(
        Expression<Func<T, bool>>? filter = null,
        Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null)
    {
        IQueryable<T> query = dbSet;

        try
        {
            if (filter != null) query = query.Where(filter);
            if (include != null) query = include(query);

            var entity = await query.FirstOrDefaultAsync();
            if (entity == null)
            {
                return DbRequest<T>.Failure($"Entity of type {typeof(T).Name} was not found.");
            }

            return DbRequest<T>.Success(entity, $"Entity with ID {entity.Id} has been retrieved successfully.");
        }
        catch (Exception e)
        {
            return DbRequest<T>.Failure(
                $"Something went wrong while retrieving an entity of type {typeof(T).Name}. \n Exception Message: {e.Message}");
        }
    }

    /// <summary>
    /// Removes an entity from the database asynchronously.
    /// </summary>
    public async Task<DbRequest> RemoveAsync(Expression<Func<T, bool>> filter)
    {
        if (filter == null)
        {
            throw new ArgumentNullException(nameof(filter), "Filter cannot be null.");
        }

        var getEntity = await GetAsync(filter);

        return await ExecuteOperationAsync(
            async () =>
            {
                if (getEntity != null && getEntity.IsSuccess && getEntity.Data != null)
                {
                    dbSet.Remove(getEntity.Data);
                    return DbRequest.Success();
                }
                else
                    return DbRequest.Failure();
            },
           successMessage: $"Entity of type {typeof(T).Name} with Id {getEntity.Data?.Id} has been deleted.",
           errorMessage: $"Something went wrong while updating entity of type {typeof(T).Name}",
           errorMessagePrefix: $"Something went wrong while deleting entity of type {typeof(T).Name}"

        );
    }
    /// <summary>
    /// Updates an entity in the database asynchronously.
    /// </summary>
    public async Task<DbRequest> UpdateAsync(T entity)
    {
        if (entity == null)
        {
            return DbRequest.Failure("Entity cannot be null");
        }

        return await ExecuteOperationAsync(
        async () =>
        {
            dbSet.Attach(entity);
            dbSet.Entry(entity).State = EntityState.Modified;
            return DbRequest.Success();
        },
        successMessage: $"Entity of type {typeof(T).Name} has been updated successfully.",
        errorMessagePrefix: $"Something went wrong while updating entity of type {typeof(T).Name}");
    }

    /// <summary>
    /// Executes a database operation asynchronously and returns a standardized response.
    /// </summary>
    private async Task<DbRequest> ExecuteOperationAsync(Func<Task<DbRequest>> operation,
                                                        string successMessage = "",
                                                        string? errorMessage = null,
                                                        string errorMessagePrefix = "")
    {
        try
        {
            var result = await operation();
            result.Message = result.IsSuccess ? successMessage :
                                                (errorMessage ?? result.Message);
            return result;
        }
        catch (Exception e)
        {
            return DbRequest.Failure($"{errorMessagePrefix} \n Exception Message: {e.Message}");
        }
    }
}

