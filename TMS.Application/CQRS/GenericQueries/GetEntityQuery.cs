namespace TMS.Application.CQRS.GenericQueries;

public record GetEntityQuery<TEntity, TEntityDto>(
    Expression<Func<TEntity, bool>>? Filter = null,
    Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? Include = null
) : IRequest<ApiResponse<TEntityDto>>
    where TEntity : Entity
    where TEntityDto : IDto;