namespace TMS.Application.Commands;
public record DeleteEntityCommand<TEntity>(Expression<Func<TEntity, bool>> Filter) : IRequest<ApiResponse> where TEntity : Entity;
public class DeleteEntityCommandHandler<TEntity>(
    IEntityCommiter entityCommiter,
    ILogger<DeleteEntityCommandHandler<TEntity>> logger)
    : IRequestHandler<DeleteEntityCommand<TEntity>, ApiResponse>
    where TEntity : Entity
{
    public async Task<ApiResponse> Handle(DeleteEntityCommand<TEntity> request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Deleteing process is starting ....");
        logger.LogInformation("check if object exist ....");
        var requestDelete = await entityCommiter.GetRepository<TEntity>().RemoveAsync(request.Filter);
        if (requestDelete.IsSuccess is false)
        {
            logger.LogError(requestDelete.Message);
            return ApiResponse.Failure(HttpStatusCode.BadRequest, requestDelete.Message);
        }
        try
        {
            await entityCommiter.CommitAsync(cancellationToken);
            logger.LogInformation(requestDelete.Message);
            return ApiResponse.Success(HttpStatusCode.OK, requestDelete.Message);
        }
        catch (Exception e)
        {
            logger.LogError(requestDelete.Message);
            return ApiResponse.Failure(HttpStatusCode.OK, e.Message);
        }
        finally
        {
            logger.LogInformation("Deleteing process is Ended ....");
        }
    }
}
