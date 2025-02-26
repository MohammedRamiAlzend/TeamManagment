namespace TMS.Application.Queries
{
    public record GetAllEntityQuery<TEntity, TEntityDTO>(
            Expression<Func<TEntity, bool>>? Filter = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? Include = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? OrderBy = null
        ) : IRequest<ApiResponse<List<TEntityDTO>>>
        where TEntity : Entity
        where TEntityDTO : IDTO;


    public class GetAllEntityQueryHandler<TEntity, TEntityDTO>(
        IEntityCommiter entityCommiter,
        IMapper mapper,
        ILogger<GetAllEntityQuery<TEntity, TEntityDTO>> logger)
        : IRequestHandler<GetAllEntityQuery<TEntity, TEntityDTO>, ApiResponse<List<TEntityDTO>>>
        where TEntity : Entity
        where TEntityDTO : IDTO
    {
        public async Task<ApiResponse<List<TEntityDTO>>> Handle(GetAllEntityQuery<TEntity, TEntityDTO> request, CancellationToken cancellationToken)
        {
            logger.LogInformation("Get All Entity is started ....");
            var requestGet = await entityCommiter.GetRepository<TEntity>().GetAllAsync(
                filter: request.Filter,
                include: request.Include,
                request.OrderBy
                );
            if (requestGet.IsSuccess is false)
            {
                logger.LogError(requestGet.Message);
                return ApiResponse<List<TEntityDTO>>.Failure(HttpStatusCode.BadRequest, requestGet.Message);
            }
            var dtoList = mapper.Map<List<TEntityDTO>>(requestGet.Data);
            logger.LogInformation("Get All Entity is finished ....");
            return ApiResponse<List<TEntityDTO>>.Success(dtoList, HttpStatusCode.OK, requestGet.Message);
        }
    }
}
