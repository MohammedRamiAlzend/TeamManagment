namespace TMS.Application.Queries;
public record GetAllPaginatedEntityQuery<TEntity>(
        Expression<Func<TEntity, bool>>? Filter = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? Include = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? OrderBy = null,
        int PageNumber = 1,
        int PageSize = 10
    ) : IRequest<DbRequest<PaginatedDbRequest<TEntity>>> where TEntity : class, IHasId;
public class GetAllPaginatedEntityQueryHandler<TEntity>(IEntityCommiter entityCommiter)
    : IRequestHandler<GetAllPaginatedEntityQuery<TEntity>, DbRequest<PaginatedDbRequest<TEntity>>>
    where TEntity : class, IHasId
{
    public async Task<DbRequest<PaginatedDbRequest<TEntity>>> Handle(GetAllPaginatedEntityQuery<TEntity> request, CancellationToken cancellationToken)
    {
        var requestGetAllPaginated = await entityCommiter.GetRepository<TEntity>().GetAllPaginatedAsync(
            filter: request.Filter,
            include: request.Include,
            orderBy: request.OrderBy,
            request.PageNumber,
            request.PageSize);

        return requestGetAllPaginated;
    }
}
