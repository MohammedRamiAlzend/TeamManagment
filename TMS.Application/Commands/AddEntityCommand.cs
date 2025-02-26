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
        logger.LogInformation("Start Adding.....");
        logger.LogInformation("Start Mapping.....");
        var entity = mapper.Map<TEntity>(request.EntityDTO);
        logger.LogInformation("End Mapping.....");
        logger.LogInformation("Adding Data.....");
        var requestAdd = await entityCommiter.GetRepository<TEntity>().AddAsync(entity);
        if (requestAdd.IsSuccess is false)
        {
            logger.LogError($"Error Accourd while adding entity of Type {typeof(TEntity)}");
            return ApiResponse<TEntityDTO>.Failure(HttpStatusCode.BadRequest, requestAdd.Message);
        }
        try
        {
            await entityCommiter.CommitAsync(cancellationToken);
            logger.LogInformation(requestAdd.Message);
            return ApiResponse<TEntityDTO>.Success(request.EntityDTO, HttpStatusCode.OK, requestAdd.Message);
        }
        catch (Exception e)
        {
            logger.LogError(requestAdd.Message);
            return ApiResponse<TEntityDTO>.Failure(HttpStatusCode.BadRequest, e.Message);
        }
        finally
        {
            logger.LogInformation("Adding process is Ended ....");
        }
    }
}