using TMS.Core.MediatR.Interfaces;

namespace TMS.Application.Queries;
public record GetAllEntityQuery<TEntity>(
        Expression<Func<TEntity, bool>>? Filter = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? Include = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? OrderBy = null
    ) : IRequest<DbRequest<List<TEntity>>> where TEntity : class, IHasId;
public class GetAllEntityQueryHandler<TEntity>(IEntityCommiter entityCommiter)
    : IRequestHandler<GetAllEntityQuery<TEntity>, DbRequest<List<TEntity>>>
    where TEntity : class, IHasId
{
    public async Task<DbRequest<List<TEntity>>> Handle(GetAllEntityQuery<TEntity> request, CancellationToken cancellationToken)
    {
        var requestGet = await entityCommiter.GetRepository<TEntity>().GetAllAsync(
            filter: request.Filter,
            include: request.Include,
            request.OrderBy
            );
        if (requestGet.IsSuccess) await entityCommiter.CommitAsync(cancellationToken);
        return requestGet;
    }
}
