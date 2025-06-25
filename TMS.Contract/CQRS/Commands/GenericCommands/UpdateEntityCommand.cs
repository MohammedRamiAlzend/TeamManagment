using Microsoft.EntityFrameworkCore.Query;

namespace TMS.Contract.CQRS.Commands.GenericCommands;

public record UpdateEntityCommand<TEntity, TEntityDto>(
    Expression<Func<TEntity, bool>> Filter,
    TEntityDto Entity,
    Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? Include = null
    ) : IRequest<ApiResponse<TEntityDto>>
    where TEntity : Entity
    where TEntityDto : IDto;