using Contracts.CQRS.GenericQueries;

namespace TMS.Application.GenericQueryHandlers;

public class GetAllEntityQueryHandler<TEntity, TEntityDto>(
    IEntityCommiter entityCommiter,
    IMapper mapper,
    ILogger<GetAllEntityQueryHandler<TEntity, TEntityDto>> logger
) : IRequestHandler<GetAllEntityQuery<TEntity, TEntityDto>, ApiResponse<List<TEntityDto>>>
    where TEntity : Entity
    where TEntityDto : IDto
{
    public async Task<ApiResponse<List<TEntityDto>>> Handle(GetAllEntityQuery<TEntity, TEntityDto> request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Processing GetAllEntityQuery for {EntityType}", typeof(TEntity).Name);

        try
        {
            var repository = entityCommiter.GetRepository<TEntity>();
            if (repository is null)
            {
                logger.LogError("Repository for {EntityType} is null", typeof(TEntity).Name);
                return ApiResponse<List<TEntityDto>>.Failure(HttpStatusCode.InternalServerError,
                    "Repository is unavailable.");
            }

            var result = await repository.GetAllAsync(request.Filter, request.Include, request.OrderBy);
            if (!result.IsSuccess)
            {
                logger.LogError("GetAll operation failed for {EntityType}: {Message}", typeof(TEntity).Name,
                    result.Message);
                return ApiResponse<List<TEntityDto>>.Failure(HttpStatusCode.BadRequest, result.Message ?? "");
            }

            var dtoList = mapper.Map<List<TEntityDto>>(result.Data);

            logger.LogInformation("GetAllEntityQuery completed successfully for {EntityType}", typeof(TEntity).Name);

            return ApiResponse<List<TEntityDto>>.Success(dtoList, HttpStatusCode.OK, result.Message ?? "");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while processing GetAllEntityQuery for {EntityType}",
                typeof(TEntity).Name);
            return ApiResponse<List<TEntityDto>>.Failure(HttpStatusCode.InternalServerError,
                "An error occurred while retrieving the entities.");
        }
    }
}