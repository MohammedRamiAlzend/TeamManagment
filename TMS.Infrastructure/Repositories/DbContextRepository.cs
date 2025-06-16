namespace TMS.Infrastructure.Repositories;

public class DbContextRepository<T>(DbSet<T> dbSet, ILogger logger) : IDbContextRepository<T>
    where T : Entity
{
    /// <summary>
    ///     Adds a new entity to the database asynchronously.
    /// </summary>
    public async Task<DbRequest> AddAsync(T entity)
    {
        if (entity == null) return DbRequest.Failure("Entity cannot be null.");

        return await ExecuteOperationAsync(
            async () =>
            {
                await dbSet.AddAsync(entity);
                return DbRequest.Success();
            },
            $"Entity of type {typeof(T).Name} has been added successfully",
            $"Failed to add entity of type {typeof(T).Name}."
        );
    }

    /// <summary>
    ///     Retrieves all entities from the database asynchronously, with optional filtering, including, and ordering.
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
            return result.Count == 0
                ? DbRequest<List<T>>.Failure($"No entities of type {typeof(T).Name} found.")
                : DbRequest<List<T>>.Success(result, "Entities retrieved successfully.");
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error retrieving entities of type {EntityType}.", typeof(T).Name);
            return DbRequest<List<T>>.Failure(
                $"Something went wrong while retrieving entities of type {typeof(T).Name}. Exception: {e.Message}");
        }
    }

    /// <summary>
    ///     Retrieves all entities from the database asynchronously with pagination, filtering, including, and ordering.
    /// </summary>
    public async Task<PaginatedDbRequest<T>> GetAllPaginatedAsync(
        Expression<Func<T, bool>>? filter = null,
        Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        int pageNumber = 1,
        int pageSize = 10)
    {
        if (pageNumber <= 0 || pageSize <= 0)
            return PaginatedDbRequest<T>.Failure("Page number and size must be greater than zero.");

        IQueryable<T> query = dbSet;

        try
        {
            if (filter != null) query = query.Where(filter);
            if (include != null) query = include(query);
            if (orderBy != null) query = orderBy(query);

            var totalCount = await query.CountAsync();
            var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

            if (items.Count == 0) return PaginatedDbRequest<T>.Failure($"No entities of type {typeof(T).Name} found.");

            return PaginatedDbRequest<T>.Success(
                items,
                totalCount,
                pageNumber,
                pageSize,
                "Entities retrieved successfully.");
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error retrieving paginated entities of type {EntityType}.", typeof(T).Name);
            return PaginatedDbRequest<T>.Failure(
                $"Something went wrong while retrieving paginated entities of type {typeof(T).Name}. Exception: {e.Message}");
        }
    }

    /// <summary>
    ///     Retrieves a single entity from the database asynchronously, with optional filtering and including related entities.
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
            return entity == null
                ? DbRequest<T>.Failure($"Entity of type {typeof(T).Name} was not found.")
                : DbRequest<T>.Success(entity, $"Entity with ID {entity.Id} has been retrieved successfully.");
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error retrieving entity of type {EntityType}.", typeof(T).Name);
            return DbRequest<T>.Failure(
                $"Something went wrong while retrieving an entity of type {typeof(T).Name}. Exception: {e.Message}");
        }
    }

    /// <summary>
    ///     Removes an entity from the database asynchronously.
    /// </summary>
    public async Task<DbRequest> RemoveAsync(Expression<Func<T, bool>> filter)
    {
        if (filter == null) return DbRequest.Failure("Filter cannot be null.");

        var getEntity = await GetAsync(filter);
        if (!getEntity.IsSuccess || getEntity.Data == null) return DbRequest.Failure("Entity not found.");

        return await ExecuteOperationAsync(
            async () =>
            {
                dbSet.Remove(getEntity.Data);
                return DbRequest.Success();
            },
            $"Entity of type {typeof(T).Name} with ID {getEntity.Data.Id} has been deleted.",
            $"Failed to delete entity of type {typeof(T).Name}."
        );
    }

    /// <summary>
    ///     Updates an entity in the database asynchronously.
    /// </summary>
    public async Task<DbRequest> UpdateAsync(T entity)
    {
        if (entity == null) return DbRequest.Failure("Entity cannot be null.");

        return await ExecuteOperationAsync(
            async () =>
            {
                dbSet.Attach(entity);
                dbSet.Entry(entity).State = EntityState.Modified;
                return DbRequest.Success();
            },
            $"Entity of type {typeof(T).Name} has been updated successfully.",
            $"Failed to update entity of type {typeof(T).Name}."
        );
    }

    /// <summary>
    ///     Executes a database operation asynchronously and returns a standardized response.
    /// </summary>
    private async Task<DbRequest> ExecuteOperationAsync(Func<Task<DbRequest>> operation,
        string successMessage = "",
        string errorMessage = "")
    {
        try
        {
            var result = await operation();
            result.Message = result.IsSuccess ? successMessage : errorMessage;
            return result;
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error during operation for entity of type {EntityType}.", typeof(T).Name);
            return DbRequest.Failure($"An error occurred during the operation. Exception: {e.Message}");
        }
    }
}