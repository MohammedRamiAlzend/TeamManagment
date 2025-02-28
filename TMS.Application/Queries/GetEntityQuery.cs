namespace TMS.Application.Queries;
public record GetEntityQuery<TEntity, TEntityDTO>(
    Expression<Func<TEntity, bool>>? Filter = null,
    Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? Include = null
) : IRequest<ApiResponse>
    where TEntity : Entity
    where TEntityDTO : IDTO;
public class GetEntityQueryHandler<TEntity, TEntityDTO>(
    IEntityCommiter entityCommiter,
    IMapper mapper,
    ILogger<GetEntityQueryHandler<TEntity, TEntityDTO>> logger
) : IRequestHandler<GetEntityQuery<TEntity, TEntityDTO>, ApiResponse>
    where TEntity : Entity
    where TEntityDTO : IDTO
{
    public async Task<ApiResponse> Handle(GetEntityQuery<TEntity, TEntityDTO> request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Processing GetEntityQuery for {EntityType}", typeof(TEntity).Name);

        try
        {
            var repository = entityCommiter.GetRepository<TEntity>();
            if (repository == null)
            {
                logger.LogError("Repository for {EntityType} is null", typeof(TEntity).Name);
                return ApiResponse.Failure(HttpStatusCode.InternalServerError, "Repository is unavailable.");
            }

            var result = await repository.GetAsync(request.Filter, request.Include);
            if (!result.IsSuccess)
            {
                logger.LogError("Get operation failed for {EntityType}: {Message}", typeof(TEntity).Name, result.Message);
                return ApiResponse.Failure(HttpStatusCode.BadRequest, result.Message);
            }

            var dto = mapper.Map<TEntityDTO>(result.Data);
            logger.LogInformation("GetEntityQuery completed successfully for {EntityType}", typeof(TEntity).Name);

            return ApiResponse.Success(dto, HttpStatusCode.OK, result.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while processing GetEntityQuery for {EntityType}", typeof(TEntity).Name);
            return ApiResponse.Failure(HttpStatusCode.InternalServerError, "An error occurred while retrieving the entity.");
        }
    }
}