

namespace TMS.Application.Queries;

public record GetEntityQuery<TEntity, TEntityDTO>(
        Expression<Func<TEntity, bool>>? Filter = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? Include = null
    ) : IRequest<ApiResponse<TEntityDTO>>
    where TEntity : Entity
    where TEntityDTO : IDTO;


public class GetEntityQueryHandler<TEntity, TEntityDTO>(
    IEntityCommiter entityCommiter,
    IMapper mapper,
    ILogger<GetEntityQuery<TEntity, TEntityDTO>> logger)
    : IRequestHandler<GetEntityQuery<TEntity, TEntityDTO>, ApiResponse<TEntityDTO>>
    where TEntity : Entity
    where TEntityDTO : IDTO
{
    public async Task<ApiResponse<TEntityDTO>> Handle(GetEntityQuery<TEntity, TEntityDTO> request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Get Entity Query is started ....");
        var requestGet = await entityCommiter.GetRepository<TEntity>().GetAsync(
            filter: request.Filter,
            include: request.Include
            );
        if (requestGet.IsSuccess is false)
        {
            logger.LogError(requestGet.Message);
            return ApiResponse<TEntityDTO>.Failure(HttpStatusCode.BadRequest, requestGet.Message);
        }
        var entityAsDTO = mapper.Map<TEntityDTO>(requestGet.Data);
        logger.LogInformation(requestGet.Message);
        logger.LogInformation("Get Entity Query is Ended ....");
        return ApiResponse<TEntityDTO>.Success(entityAsDTO, HttpStatusCode.OK, requestGet.Message);
    }
}
