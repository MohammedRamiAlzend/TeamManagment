using Contracts.CQRS.GenericQueries;
using TMS.Core.CommunicationModels;

namespace TMS.Application.Queries.GenericQueryHandlers;

public class GetAllPaginatedEntityQueryHandler<TEntity, TEntityDto>(
    IEntityCommiter entityCommiter,
    IMapper mapper,
    ILogger<GetAllPaginatedEntityQueryHandler<TEntity, TEntityDto>> logger
) : IRequestHandler<GetAllPaginatedEntityQuery<TEntity, TEntityDto>, PaginatedApiResponse<TEntityDto>>
    where TEntity : Entity
    where TEntityDto : IDto
{
    public async Task<PaginatedApiResponse<TEntityDto>> Handle(GetAllPaginatedEntityQuery<TEntity, TEntityDto> request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Processing GetAllPaginatedEntityQuery for {EntityType}", typeof(TEntity).Name);

        try
        {
            var repository = entityCommiter.GetRepository<TEntity>();
            if (repository == null)
            {
                logger.LogError("Repository for {EntityType} is null", typeof(TEntity).Name);
                return PaginatedApiResponse<TEntityDto>.Failure(HttpStatusCode.InternalServerError,
                    "Repository is unavailable.");
            }

            var result = await repository.GetAllPaginatedAsync(request.Filter, request.Include, request.OrderBy,
                request.PageNumber, request.PageSize);
            if (!result.IsSuccess)
            {
                logger.LogError("GetAllPaginated operation failed for {EntityType}: {Message}", typeof(TEntity).Name,
                    result.Message);
                return PaginatedApiResponse<TEntityDto>.Failure(HttpStatusCode.BadRequest, result.Message ?? "");
            }

            var dtoList = mapper.Map<List<TEntityDto>>(result.Items);
            logger.LogInformation("GetAllPaginatedEntityQuery completed successfully for {EntityType}",
                typeof(TEntity).Name);

            return PaginatedApiResponse<TEntityDto>.Success(
                dtoList,
                result.TotalCount,
                result.PageNumber,
                result.PageSize,
                messages: result.Message ?? ""
            );
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while processing GetAllPaginatedEntityQuery for {EntityType}",
                typeof(TEntity).Name);
            return PaginatedApiResponse<TEntityDto>.Failure(HttpStatusCode.InternalServerError,
                "An error occurred while retrieving the paginated entities.");
        }
    }
}