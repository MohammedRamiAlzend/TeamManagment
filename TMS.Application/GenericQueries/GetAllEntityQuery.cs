namespace TMS.Application.GenericQueries;

public record GetAllEntityQuery<TEntity, TEntityDto>(
    Expression<Func<TEntity, bool>>? Filter = null,
    Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? Include = null,
    Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? OrderBy = null
) : IRequest<ApiResponse>
    where TEntity : Entity
    where TEntityDto : IDto;

public class GetAllEntityQueryHandler<TEntity, TEntityDto>(
    IEntityCommiter entityCommiter,
    IMapper mapper,
    ILogger<GetAllEntityQueryHandler<TEntity, TEntityDto>> logger
) : IRequestHandler<GetAllEntityQuery<TEntity, TEntityDto>, ApiResponse>
    where TEntity : Entity
    where TEntityDto : IDto
{
    public async Task<ApiResponse> Handle(GetAllEntityQuery<TEntity, TEntityDto> request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Processing GetAllEntityQuery for {EntityType}", typeof(TEntity).Name);

        try
        {
            var repository = entityCommiter.GetRepository<TEntity>();
            if (repository == null)
            {
                logger.LogError("Repository for {EntityType} is null", typeof(TEntity).Name);
                return ApiResponse.Failure(HttpStatusCode.InternalServerError, "Repository is unavailable.");
            }

            var result = await repository.GetAllAsync(request.Filter, request.Include, request.OrderBy);
            if (!result.IsSuccess)
            {
                logger.LogError("GetAll operation failed for {EntityType}: {Message}", typeof(TEntity).Name, result.Message);
                return ApiResponse.Failure(HttpStatusCode.BadRequest, result.Message);
            }

            var dtoList = mapper.Map<List<TEntityDto>>(result.Data);
            logger.LogInformation("GetAllEntityQuery completed successfully for {EntityType}", typeof(TEntity).Name);

            return ApiResponse.Success(dtoList, HttpStatusCode.OK, result.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while processing GetAllEntityQuery for {EntityType}", typeof(TEntity).Name);
            return ApiResponse.Failure(HttpStatusCode.InternalServerError, "An error occurred while retrieving the entities.");
        }
    }
}
