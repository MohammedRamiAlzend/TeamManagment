using TMS.Application.CQRS.GenericQueries;

namespace TMS.Application.Handlers.GenericQueryHandlers;

public class GetEntityQueryHandler<TEntity, TEntityDto>(
    IEntityCommiter entityCommiter,
    IMapper mapper,
    ILogger<GetEntityQueryHandler<TEntity, TEntityDto>> logger
) : IRequestHandler<GetEntityQuery<TEntity, TEntityDto>, ApiResponse<TEntityDto>>
    where TEntity : Entity
    where TEntityDto : IDto
{
    public async Task<ApiResponse<TEntityDto>> Handle(GetEntityQuery<TEntity, TEntityDto> request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Processing GetEntityQuery for {EntityType}", typeof(TEntity).Name);

        try
        {
            var repository = entityCommiter.GetRepository<TEntity>();
            if (repository == null)
            {
                logger.LogError("Repository for {EntityType} is null", typeof(TEntity).Name);
                return ApiResponse<TEntityDto>.Failure(HttpStatusCode.InternalServerError,
                    "Repository is unavailable.");
            }

            var result = await repository.GetAsync(request.Filter, request.Include);
            if (!result.IsSuccess)
            {
                logger.LogError("Get operation failed for {EntityType}: {Message}", typeof(TEntity).Name,
                    result.Message);
                return ApiResponse<TEntityDto>.Failure(HttpStatusCode.BadRequest, result.Message ?? "");
            }

            var dto = mapper.Map<TEntityDto>(result.Data);
            logger.LogInformation("GetEntityQuery completed successfully for {EntityType}", typeof(TEntity).Name);

            return ApiResponse<TEntityDto>.Success(dto, HttpStatusCode.OK, result.Message ?? "");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while processing GetEntityQuery for {EntityType}",
                typeof(TEntity).Name);
            return ApiResponse<TEntityDto>.Failure(HttpStatusCode.InternalServerError,
                "An error occurred while retrieving the entity.");
        }
    }
}