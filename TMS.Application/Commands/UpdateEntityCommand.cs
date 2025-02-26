namespace TMS.Application.Commands;
public record UpdateEntityCommand<TEntity, TEntityDTO>(int Id, TEntityDTO Entity) :
    IRequest<ApiResponse>
    where TEntity : Entity
    where TEntityDTO : IDTO;
public class UpdateEntityCommandHandler<TEntity, TEntityDTO>(
    IEntityCommiter entityCommiter,
    IMapper mapper,
    ILogger<UpdateEntityCommandHandler<TEntity, TEntityDTO>> logger)
    : IRequestHandler<UpdateEntityCommand<TEntity, TEntityDTO>, ApiResponse>
    where TEntity : Entity
    where TEntityDTO : IDTO
{
    public async Task<ApiResponse> Handle(UpdateEntityCommand<TEntity, TEntityDTO> request, CancellationToken cancellationToken)
    {
        logger.LogInformation("start updating process ....");
        var getRepo = entityCommiter.GetRepository<TEntity>();
        logger.LogInformation("Check if entity is exist ....");
        var getEntity = await getRepo.GetAsync(x => x.Id == request.Id);
        if (getEntity.IsSuccess is false)
        {
            logger.LogWarning(getEntity.Message);
            return ApiResponse.Failure(HttpStatusCode.BadRequest,getEntity.Message);
        }
        var existingEntity = getEntity.Data;
        mapper.Map(request.Entity, existingEntity);
        var requestUpdate = await getRepo.UpdateAsync(existingEntity);
        if (requestUpdate.IsSuccess is false)
        {
            logger.LogWarning(requestUpdate.Message);
            return ApiResponse.Failure(HttpStatusCode.BadRequest,requestUpdate.Message);
        }
        try
        {
            await entityCommiter.CommitAsync(cancellationToken);
            logger.LogInformation(requestUpdate.Message);
            return ApiResponse.Success(HttpStatusCode.OK, requestUpdate.Message);
        }
        catch (Exception e)
        {
            logger.LogInformation(e.Message);
            return ApiResponse.Failure(HttpStatusCode.BadRequest,e.Message);
        }
        finally
        {
            logger.LogInformation("updating process Ended....");
        }

    }
}
