using TMS.Core.Entities.Interfaces;

namespace TMS.Core.Commands;

public record AddEntityCommand<TEntityDTO>(TEntityDTO EntityDTO) : IRequest<DbRequest> where TEntityDTO : IDTO;

public class AddEntityCommandHandler<TEntity, TEntityDTO>(
    IEntityCommiter entityCommiter,
    IMapper mapper
) : IRequestHandler<AddEntityCommand<TEntityDTO>, DbRequest>
    where TEntity : Entity
    where TEntityDTO : IDTO
{
    public async Task<DbRequest> Handle(AddEntityCommand<TEntityDTO> request, CancellationToken cancellationToken)
    {
        var entity = mapper.Map<TEntity>(request.EntityDTO);

        var requestAdd = await entityCommiter.GetRepository<TEntity>().AddAsync(entity);

        if (requestAdd.IsSuccess)
            await entityCommiter.CommitAsync(cancellationToken);

        return requestAdd;
    }
}