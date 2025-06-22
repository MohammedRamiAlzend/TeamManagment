namespace TMS.Contract.CQRS.GenericQueries;

public record GetAllPaginatedEntityQuery<TEntity, TEntityDto>(
    Expression<Func<TEntity, bool>>? Filter = null,
    Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? Include = null,
    Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? OrderBy = null,
    int PageNumber = 1,
    int PageSize = 10
) : IRequest<PaginatedApiResponse<TEntityDto>>
    where TEntity : Entity
    where TEntityDto : IDto;