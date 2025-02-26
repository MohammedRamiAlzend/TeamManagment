namespace TMS.Application.Commands;
public record AddEntityCommand<TEntityDTO>(TEntityDTO EntityDTO) : IRequest<ApiResponse<TEntityDTO>> where TEntityDTO : IDTO;
public class AddEntityCommandHandler<TEntity, TEntityDTO>(
    IEntityCommiter entityCommiter,
    IMapper mapper,
    ILogger<AddEntityCommandHandler<TEntity, TEntityDTO>> logger
) : IRequestHandler<AddEntityCommand<TEntityDTO>, ApiResponse<TEntityDTO>>
    where TEntity : Entity
    where TEntityDTO : IDTO
{
    public async Task<ApiResponse<TEntityDTO>> Handle(AddEntityCommand<TEntityDTO> request, CancellationToken cancellationToken)
    {
        if (request.EntityDTO == null)
        {
            logger.LogError("EntityDTO is null for {EntityType}", typeof(TEntityDTO).Name);
            return ApiResponse<TEntityDTO>.Failure(HttpStatusCode.UnprocessableEntity, "Invalid entity data provided.");
        }

        logger.LogInformation("Processing AddEntityCommand for {EntityType}", typeof(TEntityDTO).Name);

        TEntity entity = MapEntity(request.EntityDTO);
        if (entity == null)
        {
            return ApiResponse<TEntityDTO>.Failure(HttpStatusCode.InternalServerError, "Entity mapping resulted in null.");
        }

        return await AddEntityToRepositoryAsync(entity, request.EntityDTO, cancellationToken);
    }

    private TEntity MapEntity(TEntityDTO entityDTO)
    {
        try
        {
            logger.LogInformation("Mapping DTO to Entity for {EntityType}", typeof(TEntityDTO).Name);
            return mapper.Map<TEntity>(entityDTO);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to map DTO to Entity for {EntityType}", typeof(TEntityDTO).Name);
            throw;
        }
    }

    private async Task<ApiResponse<TEntityDTO>> AddEntityToRepositoryAsync(TEntity entity, TEntityDTO entityDTO, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Adding Entity of type {EntityType} to the repository", typeof(TEntity).Name);

            var repository = entityCommiter.GetRepository<TEntity>();
            if (repository == null)
            {
                logger.LogError("Repository for {EntityType} is null", typeof(TEntity).Name);
                return ApiResponse<TEntityDTO>.Failure(HttpStatusCode.InternalServerError, "Repository is unavailable.");
            }

            var requestAdd = await repository.AddAsync(entity);
            if (!requestAdd.IsSuccess)
            {
                logger.LogError("Repository addition failed for {EntityType}: {Message}", typeof(TEntity).Name, requestAdd.Message);
                return ApiResponse<TEntityDTO>.Failure(HttpStatusCode.BadRequest, requestAdd.Message);
            }

            await entityCommiter.CommitAsync(cancellationToken);
            logger.LogInformation("Entity added successfully: {Message}", requestAdd.Message);
            return ApiResponse<TEntityDTO>.Success(entityDTO, HttpStatusCode.Created, requestAdd.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while adding Entity of type {EntityType}", typeof(TEntity).Name);
            return ApiResponse<TEntityDTO>.Failure(HttpStatusCode.InternalServerError, "An error occurred while saving the entity.");
        }
        finally
        {
            logger.LogInformation("AddEntityCommand processing completed for {EntityType}", typeof(TEntityDTO).Name);
        }
    }
}
