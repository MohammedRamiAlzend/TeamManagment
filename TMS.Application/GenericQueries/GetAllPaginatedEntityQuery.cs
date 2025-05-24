namespace TMS.Application.GenericQueries;
public record GetAllPaginatedEntityQuery<TEntity, TEntityDto>(
    Expression<Func<TEntity, bool>>? Filter = null,
    Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? Include = null,
    Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? OrderBy = null,
    int PageNumber = 1,
    int PageSize = 10
) : IRequest<PaginatedApiResponse>
    where TEntity : Entity
    where TEntityDto : IDto;

public class GetAllPaginatedEntityQueryHandler<TEntity, TEntityDto>(
    IEntityCommiter entityCommiter,
    IMapper mapper,
    ILogger<GetAllPaginatedEntityQueryHandler<TEntity, TEntityDto>> logger
) : IRequestHandler<GetAllPaginatedEntityQuery<TEntity, TEntityDto>, PaginatedApiResponse>
    where TEntity : Entity
    where TEntityDto : IDto
{
    public async Task<PaginatedApiResponse> Handle(GetAllPaginatedEntityQuery<TEntity, TEntityDto> request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Processing GetAllPaginatedEntityQuery for {EntityType}", typeof(TEntity).Name);

        try
        {
            var repository = entityCommiter.GetRepository<TEntity>();
            if (repository == null)
            {
                logger.LogError("Repository for {EntityType} is null", typeof(TEntity).Name);
                return PaginatedApiResponse.Failure(HttpStatusCode.InternalServerError, "Repository is unavailable.");
            }

            var result = await repository.GetAllPaginatedAsync(request.Filter, request.Include, request.OrderBy, request.PageNumber, request.PageSize);
            if (!result.IsSuccess)
            {
                logger.LogError("GetAllPaginated operation failed for {EntityType}: {Message}", typeof(TEntity).Name, result.Message);
                return PaginatedApiResponse.Failure(HttpStatusCode.BadRequest, result.Message);
            }

            var dtoList = mapper.Map<List<TEntityDto>>((List<TEntity>)result.Data);
            logger.LogInformation("GetAllPaginatedEntityQuery completed successfully for {EntityType}", typeof(TEntity).Name);

            return PaginatedApiResponse.Success(
                items: dtoList,
                totalCount: result.TotalCount,
                pageNumber: result.PageNumber,
                pageSize: result.PageSize,
                messages: result.Message
            );
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while processing GetAllPaginatedEntityQuery for {EntityType}", typeof(TEntity).Name);
            return PaginatedApiResponse.Failure(HttpStatusCode.InternalServerError, "An error occurred while retrieving the paginated entities.");
        }
    }
}
