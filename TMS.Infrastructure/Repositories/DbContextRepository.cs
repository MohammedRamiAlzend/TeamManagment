using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Logging;

namespace TMS.Infrastructure.Repositories;

public class DbContextRepository<T>(DbSet<T> dbSet, ILogger<DbContextRepository<T>> logger) : IDbContextRepository<T>
    where T : class, IHasId
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
            async () => await dbSet.AddAsync(entity),
            $"Entity of type {typeof(T).Name} has been added successfully",
            $"Something went wrong while adding entity of type {typeof(T).Name}"
        );
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
            logger.LogError(e, "An error occurred while retrieving entities.");
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
            logger.LogError(e, "An error occurred while retrieving paginated entities.");
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
            logger.LogError(e, "An error occurred while retrieving an entity.");
            return DbRequest<T>.Failure(
                $"Something went wrong while retrieving an entity of type {typeof(T).Name}. \n Exception Message: {e.Message}");
        }
    }

    /// <summary>
    /// Removes an entity from the database asynchronously.
    /// </summary>
    public async Task<DbRequest> RemoveAsync(T entity)
    {
        if (entity == null)
        {
            return DbRequest.Failure("Entity cannot be null");
        }

        return await ExecuteOperationAsync(
            () =>
            {
                dbSet.Remove(entity);
                return Task.CompletedTask;
            },
            $"Entity of type {typeof(T).Name} with ID {entity.Id} has been deleted.",
            $"Something went wrong while deleting entity of type {typeof(T).Name}"
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
            () =>
            {
                dbSet.Attach(entity);
                dbSet.Entry(entity).State = EntityState.Modified;
                return Task.CompletedTask;
            },
            "Entity has been updated successfully.",
            $"Something went wrong while updating entity of type {typeof(T).Name}"
        );
    }

    /// <summary>
    /// Executes a database operation asynchronously and returns a standardized response.
    /// </summary>
    private async Task<DbRequest> ExecuteOperationAsync(Func<Task> operation, string successMessage, string errorMessagePrefix)
    {
        try
        {
            await operation();
            return DbRequest.Success(successMessage);
        }
        catch (Exception e)
        {
            logger.LogError(e, "An error occurred during the operation.");
            return DbRequest.Failure($"{errorMessagePrefix} \n Exception Message: {e.Message}");
        }
    }
}

