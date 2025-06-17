namespace TMS.Application.CQRS.GenericCommands;

public record UpdateEntityCommand<TEntity, TEntityDto>(
    int Id,
    TEntityDto Entity,
    Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? Include = null
    ) : IRequest<ApiResponse<TEntityDto>>
    where TEntity : Entity
    where TEntityDto : IDto;