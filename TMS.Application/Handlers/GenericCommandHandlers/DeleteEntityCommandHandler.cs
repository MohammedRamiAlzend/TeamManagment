namespace TMS.Application.Handlers.GenericCommandHandlers;

public class DeleteEntityCommandHandler<TEntity>(
    IEntityCommiter entityCommiter,
    ILogger<DeleteEntityCommandHandler<TEntity>> logger
) : IRequestHandler<DeleteEntityCommand<TEntity>, ApiResponse>
    where TEntity : Entity
{
    public async Task<ApiResponse> Handle(DeleteEntityCommand<TEntity> request, CancellationToken cancellationToken)
    {
        if (request.Filter == null)
        {
            logger.LogError("Filter expression is null for {EntityType}", typeof(TEntity).Name);
            return ApiResponse.Failure(HttpStatusCode.BadRequest, "Invalid filter provided.");
        }

        logger.LogInformation("Processing DeleteEntityCommand for {EntityType}", typeof(TEntity).Name);

        try
        {
            var repository = entityCommiter.GetRepository<TEntity>();
            if (repository == null)
            {
                logger.LogError("Repository for {EntityType} is null", typeof(TEntity).Name);
                return ApiResponse.Failure(HttpStatusCode.InternalServerError, "Repository is unavailable.");
            }

            var result = await repository.RemoveAsync(request.Filter);
            if (!result.IsSuccess)
            {
                logger.LogError("Delete operation failed for {EntityType}: {Message}", typeof(TEntity).Name,
                    result.Message);
                return ApiResponse.Failure(HttpStatusCode.BadRequest, result.Message);
            }

            await entityCommiter.CommitAsync(cancellationToken);
            logger.LogInformation("Entity deleted successfully: {Message}", result.Message);

            return ApiResponse.Success(HttpStatusCode.OK, result.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while deleting Entity of type {EntityType}", typeof(TEntity).Name);
            return ApiResponse.Failure(HttpStatusCode.InternalServerError,
                "An error occurred while deleting the entity.");
        }
        finally
        {
            logger.LogInformation("DeleteEntityCommand processing completed for {EntityType}", typeof(TEntity).Name);
        }
    }
}