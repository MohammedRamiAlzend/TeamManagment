namespace TMS.Application.GenericCommandHandlers;

public class AddEntityCommandHandler<TEntity, TEntityDto>(
    IEntityCommiter entityCommiter,
    IMapper mapper,
    ILogger<AddEntityCommandHandler<TEntity, TEntityDto>> logger
) : IRequestHandler<AddEntityCommand<TEntityDto>, ApiResponse<TEntityDto>>
    where TEntity : Entity
    where TEntityDto : IDto
{
    public async Task<ApiResponse<TEntityDto>> Handle(AddEntityCommand<TEntityDto> request,
        CancellationToken cancellationToken)
    {
        if (request.EntityDto == null)
        {
            logger.LogError("EntityDTO is null for {EntityType}", typeof(TEntityDto).Name);
            return ApiResponse<TEntityDto>.Failure(HttpStatusCode.UnprocessableEntity, "Invalid entity data provided.");
        }

        logger.LogInformation("Processing AddEntityCommand for {EntityType}", typeof(TEntityDto).Name);

        var entity = MapEntity(request.EntityDto);
        if (entity == null)
            return ApiResponse<TEntityDto>.Failure(HttpStatusCode.InternalServerError,
                "Entity mapping resulted in null.");

        return await AddEntityToRepositoryAsync(entity, request.EntityDto, cancellationToken);
    }

    private TEntity MapEntity(TEntityDto entityDto)
    {
        try
        {
            logger.LogInformation("Mapping DTO to Entity for {EntityType}", typeof(TEntityDto).Name);
            return mapper.Map<TEntity>(entityDto);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to map DTO to Entity for {EntityType}", typeof(TEntityDto).Name);
            throw;
        }
    }

    private async Task<ApiResponse<TEntityDto>> AddEntityToRepositoryAsync(TEntity entity, TEntityDto entityDto,
        CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Adding Entity of type {EntityType} to the repository", typeof(TEntity).Name);

            var repository = entityCommiter.GetRepository<TEntity>();
            if (repository == null)
            {
                logger.LogError("Repository for {EntityType} is null", typeof(TEntity).Name);
                return ApiResponse<TEntityDto>.Failure(HttpStatusCode.InternalServerError,
                    "Repository is unavailable.");
            }

            var requestAdd = await repository.AddAsync(entity);
            if (!requestAdd.IsSuccess)
            {
                logger.LogError("Repository addition failed for {EntityType}: {Message}", typeof(TEntity).Name,
                    requestAdd.Message);
                return ApiResponse<TEntityDto>.Failure(HttpStatusCode.BadRequest, requestAdd.Message);
            }

            await entityCommiter.CommitAsync(cancellationToken);
            logger.LogInformation("Entity added successfully: {Message}", requestAdd.Message);
            return ApiResponse<TEntityDto>.Success(entityDto, HttpStatusCode.Created, requestAdd.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while adding Entity of type {EntityType}", typeof(TEntity).Name);
            return ApiResponse<TEntityDto>.Failure(HttpStatusCode.InternalServerError,
                "An error occurred while saving the entity.");
        }
        finally
        {
            logger.LogInformation("AddEntityCommand processing completed for {EntityType}", typeof(TEntityDto).Name);
        }
    }
}