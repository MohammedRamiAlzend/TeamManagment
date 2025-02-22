using AutoMapper;
using TMS.Core.AutoMapperClasses.DTOs;
using TMS.Core.MediatR.Interfaces;

namespace TMS.Application.Queries;
public record GetAllPaginatedEntityQuery<TEntity, TEntityDTO>(
        Expression<Func<TEntity, bool>>? Filter = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? Include = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? OrderBy = null,
        int PageNumber = 1,
        int PageSize = 10
    ) : IRequest<DbRequest<PaginatedDbRequest<TEntityDTO>>>
    where TEntity : Entity
    where TEntityDTO : IDTO;
public class GetAllPaginatedEntityQueryHandler<TEntity, TEntityDTO>(
    IEntityCommiter entityCommiter,
    IMapper mapper)
    : IRequestHandler<GetAllPaginatedEntityQuery<TEntity, TEntityDTO>, DbRequest<PaginatedDbRequest<TEntityDTO>>>
    where TEntity : Entity
    where TEntityDTO : IDTO
{
    public async Task<DbRequest<PaginatedDbRequest<TEntityDTO>>> Handle(GetAllPaginatedEntityQuery<TEntity, TEntityDTO> request, CancellationToken cancellationToken)
    {
        var requestGetAllPaginated = await entityCommiter.GetRepository<TEntity>().GetAllPaginatedAsync(
            filter: request.Filter,
            include: request.Include,
            orderBy: request.OrderBy,
            request.PageNumber,
            request.PageSize);

        var dtoPaginatedList = mapper.Map<List<TEntityDTO>>(requestGetAllPaginated.Data.Items);
        return new DbRequest<PaginatedDbRequest<TEntityDTO>>
        {
            Data = new PaginatedDbRequest<TEntityDTO>
            {
                Items = dtoPaginatedList,
                PageNumber = requestGetAllPaginated.Data.PageNumber,
                PageSize = requestGetAllPaginated.Data.PageSize,
                TotalCount= requestGetAllPaginated.Data.TotalCount
            }
        };
    }
}
