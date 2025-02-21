
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Logging;

namespace TMS.Infrastructure.Repositories;

public class DbContextRepository<T>(DbSet<T> dbSet ,ILogger<DbContextRepository<T>> logger) : IDbContextRepository<T> where T : class, IHasId
{
    /// <summary>
    /// Adds a new entity to the database asynchronously.
    /// </summary>
    /// <param name="entity">The entity to add.</param>
    /// <returns>A <see cref="DbRequest"/> indicating the result of the operation.</returns>
    public async Task<DbRequest> AddAsync(T entity)
    {
        if (entity == null)
        {
            return new DbRequest
            {
                IsSuccess = false,
                Message = "Entity cannot be null"
            };
        }

        return await ExecuteOperationAsync(
            async () => await dbSet.AddAsync(entity),
            $"Entity of type {nameof(T)} has been added successfully",
            $"Something went wrong while adding entity of type {nameof(T)}"
        );
    }

    /// <summary>
    /// Retrieves all entities from the database asynchronously, with optional filtering, including, and ordering.
    /// </summary>
    /// <param name="filter">Optional filter expression.</param>
    /// <param name="include">Optional include expression for related entities.</param>
    /// <param name="orderBy">Optional order-by expression.</param>
    /// <returns>A <see cref="DbRequest{T}"/> containing the retrieved entities or an error message.</returns>
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
                return new DbRequest<List<T>>
                {
                    IsSuccess = false,
                    Message = $"No entities of type {typeof(T)} found."
                };
            }

            return new DbRequest<List<T>>
            {
                Data = result,
                IsSuccess = true,
                Message = "Entities retrieved successfully."
            };
        }
        catch (Exception e)
        {
            logger.LogError(e, "An error occurred while retrieving entities.");
            return new DbRequest<List<T>>
            {
                IsSuccess = false,
                Message = $"Something went wrong while retrieving entities of type {nameof(T)}. \n Exception Message: {e.Message}"
            };
        }
    }

    /// <summary>
    /// Retrieves a single entity from the database asynchronously, with optional filtering and including related entities.
    /// </summary>
    /// <param name="filter">Optional filter expression.</param>
    /// <param name="include">Optional include expression for related entities.</param>
    /// <returns>A <see cref="DbRequest{T}"/> containing the retrieved entity or an error message.</returns>
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
                return new DbRequest<T>
                {
                    IsSuccess = false,
                    Message = $"Entity of type {nameof(T)} was not found."
                };
            }

            return new DbRequest<T>
            {
                Data = entity,
                IsSuccess = true,
                Message = $"Entity with ID {entity.Id} has been retrieved successfully."
            };
        }
        catch (Exception e)
        {
            logger.LogError(e, "An error occurred while retrieving an entity.");
            return new DbRequest<T>
            {
                IsSuccess = false,
                Message = $"Something went wrong while retrieving an entity of type {nameof(T)}. \n Exception Message: {e.Message}"
            };
        }
    }

    /// <summary>
    /// Removes an entity from the database asynchronously.
    /// </summary>
    /// <param name="entity">The entity to remove.</param>
    /// <returns>A <see cref="DbRequest"/> indicating the result of the operation.</returns>
    public async Task<DbRequest> RemoveAsync(T entity)
    {
        if (entity == null)
        {
            return new DbRequest
            {
                IsSuccess = false,
                Message = "Entity cannot be null"
            };
        }

        return await ExecuteOperationAsync(
            () =>
            {
                dbSet.Remove(entity);
                return Task.CompletedTask;
            },
            $"Entity of type {typeof(T)} with ID {entity.Id} has been deleted.",
            $"Something went wrong while deleting entity of type {nameof(T)}"
        );
    }

    /// <summary>
    /// Updates an entity in the database asynchronously.
    /// </summary>
    /// <param name="entity">The entity to update.</param>
    /// <returns>A <see cref="DbRequest"/> indicating the result of the operation.</returns>
    public async Task<DbRequest> UpdateAsync(T entity)
    {
        if (entity == null)
        {
            return new DbRequest
            {
                IsSuccess = false,
                Message = "Entity cannot be null"
            };
        }

        return await ExecuteOperationAsync(
            () =>
            {
                dbSet.Attach(entity);
                dbSet.Entry(entity).State = EntityState.Modified;
                return Task.CompletedTask;
            },
            "Entity has been updated successfully.",
            $"Something went wrong while updating entity of type {nameof(T)}"
        );
    }

    /// <summary>
    /// Executes a database operation asynchronously and returns a standardized response.
    /// </summary>
    /// <param name="operation">The operation to execute.</param>
    /// <param name="successMessage">The success message to return if the operation succeeds.</param>
    /// <param name="errorMessagePrefix">The error message prefix to return if the operation fails.</param>
    /// <returns>A <see cref="DbRequest"/> indicating the result of the operation.</returns>
    private async Task<DbRequest> ExecuteOperationAsync(Func<Task> operation, string successMessage, string errorMessagePrefix)
    {
        try
        {
            await operation();
            return new DbRequest
            {
                IsSuccess = true,
                Message = successMessage
            };
        }
        catch (Exception e)
        {
            logger.LogError(e, "An error occurred during the operation.");
            return new DbRequest
            {
                IsSuccess = false,
                Message = $"{errorMessagePrefix} \n Exception Message: {e.Message}"
            };
        }
    }
}
