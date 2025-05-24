using Azure.Core;

namespace TMS.Application.GenericCommands;
public record UpdateEntityCommand<TEntity, TEntityDto>(int Id, TEntityDto Entity) : IRequest<ApiResponse>
    where TEntity : Entity
    where TEntityDto : IDto;

public class UpdateEntityCommandHandler<TEntity, TEntityDto>(
    IEntityCommiter entityCommiter,
    IMapper mapper,
    ILogger<UpdateEntityCommandHandler<TEntity, TEntityDto>> logger
) : IRequestHandler<UpdateEntityCommand<TEntity, TEntityDto>, ApiResponse>
    where TEntity : Entity
    where TEntityDto : IDto
{
    public async Task<ApiResponse> Handle(UpdateEntityCommand<TEntity, TEntityDto> request, CancellationToken cancellationToken)
    {
        if (request.Entity == null)
        {
            logger.LogError("EntityDTO is null for {EntityType}", typeof(TEntityDto).Name);
            return ApiResponse.Failure(HttpStatusCode.BadRequest, "Invalid entity data provided.");
        }

        logger.LogInformation("Processing UpdateEntityCommand for {EntityType}", typeof(TEntityDto).Name);

        try
        {
            var repository = entityCommiter.GetRepository<TEntity>();
            if (repository == null)
            {
                logger.LogError("Repository for {EntityType} is null", typeof(TEntity).Name);
                return ApiResponse.Failure(HttpStatusCode.InternalServerError, "Repository is unavailable.");
            }

            var existingEntityResult = await repository.GetAsync(x => x.Id == request.Id);
            if (!existingEntityResult.IsSuccess)
            {
                logger.LogWarning("Entity not found for update: {Message}", existingEntityResult.Message);
                return ApiResponse.Failure(HttpStatusCode.BadRequest, existingEntityResult.Message);
            }

            var existingEntity = existingEntityResult.Data;
            mapper.Map(request.Entity, existingEntity);

            var updateResult = await repository.UpdateAsync((TEntity)existingEntity);
            if (!updateResult.IsSuccess)
            {
                logger.LogWarning("Update failed for {EntityType}: {Message}", typeof(TEntity).Name, updateResult.Message);
                return ApiResponse.Failure(HttpStatusCode.BadRequest, updateResult.Message);
            }

            var saveResult = await entityCommiter.CommitAsync(cancellationToken);
            if (saveResult > 0)
            {
                logger.LogInformation("Entity updated successfully: {Message}", updateResult.Message);
                return ApiResponse.Success(HttpStatusCode.OK, updateResult.Message);
            }
            else
            {
                return ApiResponse.Failure(HttpStatusCode.BadRequest, "Error Accourd while SaveChanges");
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while updating Entity of type {EntityType}", typeof(TEntity).Name);
            return ApiResponse.Failure(HttpStatusCode.InternalServerError, "An error occurred while updating the entity.");
        }
        finally
        {
            logger.LogInformation("UpdateEntityCommand processing completed for {EntityType}", typeof(TEntityDto).Name);
        }
    }
}
