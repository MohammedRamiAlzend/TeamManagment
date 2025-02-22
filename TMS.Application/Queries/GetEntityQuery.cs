using Microsoft.Extensions.Logging;
using TMS.Core.MediatR.Interfaces;

namespace TMS.Application.Queries;
public record GetEntityQuery<TEntity>(
        Expression<Func<TEntity, bool>>? Filter = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? Include = null
    ) : IRequest<DbRequest<TEntity>> where TEntity :  Entity;
public class GetEntityQueryHandler<TEntity>(IEntityCommiter entityCommiter,ILogger<TEntity> logger)
    : IRequestHandler<GetEntityQuery<TEntity>, DbRequest<TEntity>>
    where TEntity : Entity
{
    public async Task<DbRequest<TEntity>> Handle(GetEntityQuery<TEntity> request, CancellationToken cancellationToken)
    {
        var requestGet = await entityCommiter.GetRepository<TEntity>().GetAsync(
            filter: request.Filter,
            include: request.Include
            );
        return requestGet;
    }
}
