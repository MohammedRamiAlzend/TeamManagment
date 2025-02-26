namespace TMS.Application.Queries;

public record GetAllEntityQuery<TEntity, TEntityDTO>(
    Expression<Func<TEntity, bool>>? Filter = null,
    Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? Include = null,
    Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? OrderBy = null
) : IRequest<ApiResponse<List<TEntityDTO>>>
    where TEntity : Entity
    where TEntityDTO : IDTO;

public class GetAllEntityQueryHandler<TEntity, TEntityDTO>(
    IEntityCommiter entityCommiter,
    IMapper mapper,
    ILogger<GetAllEntityQueryHandler<TEntity, TEntityDTO>> logger
) : IRequestHandler<GetAllEntityQuery<TEntity, TEntityDTO>, ApiResponse<List<TEntityDTO>>>
    where TEntity : Entity
    where TEntityDTO : IDTO
{
    public async Task<ApiResponse<List<TEntityDTO>>> Handle(GetAllEntityQuery<TEntity, TEntityDTO> request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Processing GetAllEntityQuery for {EntityType}", typeof(TEntity).Name);

        try
        {
            var repository = entityCommiter.GetRepository<TEntity>();
            if (repository == null)
            {
                logger.LogError("Repository for {EntityType} is null", typeof(TEntity).Name);
                return ApiResponse<List<TEntityDTO>>.Failure(HttpStatusCode.InternalServerError, "Repository is unavailable.");
            }

            var result = await repository.GetAllAsync(request.Filter, request.Include, request.OrderBy);
            if (!result.IsSuccess)
            {
                logger.LogError("GetAll operation failed for {EntityType}: {Message}", typeof(TEntity).Name, result.Message);
                return ApiResponse<List<TEntityDTO>>.Failure(HttpStatusCode.BadRequest, result.Message);
            }

            var dtoList = mapper.Map<List<TEntityDTO>>(result.Data);
            logger.LogInformation("GetAllEntityQuery completed successfully for {EntityType}", typeof(TEntity).Name);

            return ApiResponse<List<TEntityDTO>>.Success(dtoList, HttpStatusCode.OK, result.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while processing GetAllEntityQuery for {EntityType}", typeof(TEntity).Name);
            return ApiResponse<List<TEntityDTO>>.Failure(HttpStatusCode.InternalServerError, "An error occurred while retrieving the entities.");
        }
    }
}
