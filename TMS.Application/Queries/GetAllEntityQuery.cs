using TMS.Core.Entities.Interfaces;

namespace TMS.Core.Queries
{
    public record GetAllEntityQuery<TEntity,TEntityDTO>(
            Expression<Func<TEntity, bool>>? Filter = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? Include = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? OrderBy = null
        ) : IRequest<DbRequest<List<TEntityDTO>>>
        where TEntity : Entity
        where TEntityDTO : IDTO;


    public class GetAllEntityQueryHandler<TEntity, TEntityDTO>(
        IEntityCommiter entityCommiter,
        IMapper mapper)
        : IRequestHandler<GetAllEntityQuery<TEntity,TEntityDTO>, DbRequest<List<TEntityDTO>>>
        where TEntity : Entity
        where TEntityDTO : IDTO
    {
        public async Task<DbRequest<List<TEntityDTO>>> Handle(GetAllEntityQuery<TEntity,TEntityDTO> request, CancellationToken cancellationToken)
        {
            var requestGet = await entityCommiter.GetRepository<TEntity>().GetAllAsync(
                filter: request.Filter,
                include: request.Include,
                request.OrderBy
                );
            var dtoList = mapper.Map<List<TEntityDTO>>(requestGet.Data);
            return new DbRequest<List<TEntityDTO>> {  Data = dtoList };
        }
    }
}
