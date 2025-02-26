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
    ILogger<GetAllPaginatedEntityQueryHandler<TEntity, TEntityDTO>> logger
) : IRequestHandler<GetAllPaginatedEntityQuery<TEntity, TEntityDTO>, PaginatedApiResponse<TEntityDTO>>
    where TEntity : Entity
    where TEntityDTO : IDTO
{
    public async Task<PaginatedApiResponse<TEntityDTO>> Handle(GetAllPaginatedEntityQuery<TEntity, TEntityDTO> request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Processing GetAllPaginatedEntityQuery for {EntityType}", typeof(TEntity).Name);

        try
        {
            var repository = entityCommiter.GetRepository<TEntity>();
            if (repository == null)
            {
                logger.LogError("Repository for {EntityType} is null", typeof(TEntity).Name);
                return PaginatedApiResponse<TEntityDTO>.Failure(HttpStatusCode.InternalServerError, "Repository is unavailable.");
            }

            var result = await repository.GetAllPaginatedAsync(request.Filter, request.Include, request.OrderBy, request.PageNumber, request.PageSize);
            if (!result.IsSuccess)
            {
                logger.LogError("GetAllPaginated operation failed for {EntityType}: {Message}", typeof(TEntity).Name, result.Message);
                return PaginatedApiResponse<TEntityDTO>.Failure(HttpStatusCode.BadRequest, result.Message);
            }

            var dtoList = mapper.Map<List<TEntityDTO>>(result.Data.Items);
            logger.LogInformation("GetAllPaginatedEntityQuery completed successfully for {EntityType}", typeof(TEntity).Name);

            return PaginatedApiResponse<TEntityDTO>.Success(
                items: dtoList,
                totalCount: result.Data.TotalCount,
                pageNumber: result.Data.PageNumber,
                pageSize: result.Data.PageSize,
                code: HttpStatusCode.OK,
                messages: result.Message
            );
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while processing GetAllPaginatedEntityQuery for {EntityType}", typeof(TEntity).Name);
            return PaginatedApiResponse<TEntityDTO>.Failure(HttpStatusCode.InternalServerError, "An error occurred while retrieving the paginated entities.");
        }
    }
}
