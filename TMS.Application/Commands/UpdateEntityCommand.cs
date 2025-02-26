using Azure.Core;

namespace TMS.Application.Commands;
public record UpdateEntityCommand<TEntity, TEntityDTO>(int Id, TEntityDTO Entity) : IRequest<ApiResponse>
    where TEntity : Entity
    where TEntityDTO : IDTO;

public class UpdateEntityCommandHandler<TEntity, TEntityDTO>(
    IEntityCommiter entityCommiter,
    IMapper mapper,
    ILogger<UpdateEntityCommandHandler<TEntity, TEntityDTO>> logger
) : IRequestHandler<UpdateEntityCommand<TEntity, TEntityDTO>, ApiResponse>
    where TEntity : Entity
    where TEntityDTO : IDTO
{
    public async Task<ApiResponse> Handle(UpdateEntityCommand<TEntity, TEntityDTO> request, CancellationToken cancellationToken)
    {
        if (request.Entity == null)
        {
            logger.LogError("EntityDTO is null for {EntityType}", typeof(TEntityDTO).Name);
            return ApiResponse.Failure(HttpStatusCode.BadRequest, "Invalid entity data provided.");
        }

        logger.LogInformation("Processing UpdateEntityCommand for {EntityType}", typeof(TEntityDTO).Name);

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

            var existingEntity =  existingEntityResult.Data;
            mapper.Map(request.Entity, existingEntity);
            
            var updateResult = await repository.UpdateAsync(existingEntity);
            if (!updateResult.IsSuccess)
            {
                logger.LogWarning("Update failed for {EntityType}: {Message}", typeof(TEntity).Name, updateResult.Message);
                return ApiResponse.Failure(HttpStatusCode.BadRequest, updateResult.Message);
            }

            await entityCommiter.CommitAsync(cancellationToken);
            logger.LogInformation("Entity updated successfully: {Message}", updateResult.Message);

            return ApiResponse.Success(HttpStatusCode.OK, updateResult.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while updating Entity of type {EntityType}", typeof(TEntity).Name);
            return ApiResponse.Failure(HttpStatusCode.InternalServerError, "An error occurred while updating the entity.");
        }
        finally
        {
            logger.LogInformation("UpdateEntityCommand processing completed for {EntityType}", typeof(TEntityDTO).Name);
        }
    }
}
