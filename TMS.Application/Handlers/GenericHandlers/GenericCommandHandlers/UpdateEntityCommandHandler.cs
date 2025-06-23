namespace TMS.Application.Handlers.GenericHandlers.GenericCommandHandlers;

public class UpdateEntityCommandHandler<TEntity, TEntityDto>(
    IEntityCommiter entityCommiter,
    IMapper mapper,
    ILogger<UpdateEntityCommandHandler<TEntity, TEntityDto>> logger
) : IRequestHandler<UpdateEntityCommand<TEntity, TEntityDto>, ApiResponse<TEntityDto>>
    where TEntity : Entity
    where TEntityDto : IDto
{
    public async Task<ApiResponse<TEntityDto>> Handle(UpdateEntityCommand<TEntity, TEntityDto> request,
        CancellationToken cancellationToken)
    {
        if (request.Entity == null)
        {
            logger.LogError("EntityDTO is null for {EntityType}", typeof(TEntityDto).Name);
            return ApiResponse<TEntityDto>.Failure(HttpStatusCode.BadRequest, "Invalid entity data provided.");
        }

        logger.LogInformation("Processing UpdateEntityCommand for {EntityType}", typeof(TEntityDto).Name);

        try
        {
            var repository = entityCommiter.GetRepository<TEntity>();
            if (repository == null)
            {
                logger.LogError("Repository for {EntityType} is null", typeof(TEntity).Name);
                return ApiResponse<TEntityDto>.Failure(HttpStatusCode.InternalServerError,
                    "Repository is unavailable.");
            }

            var existingEntityResult = await repository.GetAsync(request.Filter,include:request.Include);
            if (!existingEntityResult.IsSuccess)
            {
                logger.LogWarning("Entity not found for update: {Message}", existingEntityResult.Message);
                return ApiResponse<TEntityDto>.Failure(HttpStatusCode.BadRequest, existingEntityResult.Message ?? "");
            }

            var existingEntity = existingEntityResult.Data;
            mapper.Map(request.Entity, existingEntity);

            var updateResult = await repository.UpdateAsync((TEntity)existingEntity);
            if (!updateResult.IsSuccess)
            {
                logger.LogWarning("Update failed for {EntityType}: {Message}", typeof(TEntity).Name,
                    updateResult.Message);
                return ApiResponse<TEntityDto>.Failure(HttpStatusCode.BadRequest, updateResult.Message ?? "");
            }

            var saveResult = await entityCommiter.CommitAsync(cancellationToken);
            if (saveResult > 0)
            {
                logger.LogInformation("Entity updated successfully: {Message}", updateResult.Message);
                return ApiResponse<TEntityDto>.Success(HttpStatusCode.OK, updateResult.Message ?? "");
            }
            else
            {
                return ApiResponse<TEntityDto>.Failure(HttpStatusCode.BadRequest, "Error Accourd while SaveChanges");
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while updating Entity of type {EntityType}", typeof(TEntity).Name);
            return ApiResponse<TEntityDto>.Failure(HttpStatusCode.InternalServerError,
                "An error occurred while updating the entity.");
        }
        finally
        {
            logger.LogInformation("UpdateEntityCommand processing completed for {EntityType}", typeof(TEntityDto).Name);
        }
    }
}