using AutoMapper;
using Microsoft.Extensions.Logging;
using TMS.Core.AutoMapperClasses.DTOs;
using TMS.Core.MediatR.Interfaces;

namespace TMS.Application.Queries;
public record GetEntityQuery<TEntity, TEntityDTO>(
        Expression<Func<TEntity, bool>>? Filter = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? Include = null
    ) : IRequest<DbRequest<TEntityDTO>>
    where TEntity : Entity
    where TEntityDTO : IDTO;
public class GetEntityQueryHandler<TEntity, TEntityDTO>(
    IEntityCommiter entityCommiter,
    IMapper mapper)
    : IRequestHandler<GetEntityQuery<TEntity, TEntityDTO>, DbRequest<TEntityDTO>>
    where TEntity : Entity
    where TEntityDTO : IDTO
{
    public async Task<DbRequest<TEntityDTO>> Handle(GetEntityQuery<TEntity, TEntityDTO> request, CancellationToken cancellationToken)
    {
        var requestGet = await entityCommiter.GetRepository<TEntity>().GetAsync(
            filter: request.Filter,
            include: request.Include
            );
        var x = mapper.Map<TEntityDTO>(requestGet.Data);

        return new DbRequest<TEntityDTO>
        {
            Data = x
        };
    }


}
