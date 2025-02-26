namespace TMS.Application.Queries;
public record GetAllPaginatedEntityQuery<TEntity, TEntityDTO>(
        Expression<Func<TEntity, bool>>? Filter = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? Include = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? OrderBy = null,
        int PageNumber = 1,
        int PageSize = 10
    ) : IRequest<PaginatedApiResponse<TEntityDTO>>
    where TEntity : Entity
    where TEntityDTO : IDTO;
public class GetAllPaginatedEntityQueryHandler<TEntity, TEntityDTO>(
    IEntityCommiter entityCommiter,
    IMapper mapper,
    ILogger<GetAllPaginatedEntityQuery<TEntity, TEntityDTO>> logger)
    : IRequestHandler<GetAllPaginatedEntityQuery<TEntity, TEntityDTO>, PaginatedApiResponse<TEntityDTO>>
    where TEntity : Entity
    where TEntityDTO : IDTO
{
    public async Task<PaginatedApiResponse<TEntityDTO>> Handle(GetAllPaginatedEntityQuery<TEntity, TEntityDTO> request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Get All Entity Paginated is started ....");

        var requestGetAllPaginated = await entityCommiter.GetRepository<TEntity>().GetAllPaginatedAsync(
            filter: request.Filter,
            include: request.Include,
            orderBy: request.OrderBy,
            request.PageNumber,
            request.PageSize);
        if(requestGetAllPaginated.IsSuccess is false)
        {
            logger.LogError(requestGetAllPaginated.Message);
            return PaginatedApiResponse<TEntityDTO>.Failure(HttpStatusCode.BadRequest, requestGetAllPaginated.Message);
        }
        var dtoPaginatedList = mapper.Map<List<TEntityDTO>>(requestGetAllPaginated.Data.Items);
        logger.LogInformation("Get All Entity Paginated is Ended ....");

        return PaginatedApiResponse<TEntityDTO>.Success(
                    items: dtoPaginatedList,
                    totalCount: requestGetAllPaginated.Data.TotalCount,
                    pageNumber: requestGetAllPaginated.Data.PageNumber,
                    pageSize: requestGetAllPaginated.Data.PageSize,
                    code: HttpStatusCode.OK,
                    messages: requestGetAllPaginated.Message);
    }
}
