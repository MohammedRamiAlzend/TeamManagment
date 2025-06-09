using TMS.Core.CommunicationModels;

namespace Contracts.CQRS.GenericQueries;

public record GetAllEntityQuery<TEntity, TEntityDto>(
    Expression<Func<TEntity, bool>>? Filter = null,
    Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? Include = null,
    Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? OrderBy = null
) : IRequest<ApiResponse<List<TEntityDto>>>
    where TEntity : Entity
    where TEntityDto : IDto;