using AutoMapper;
using TMS.Core.AutoMapperClasses.DTOs;
using TMS.Core.MediatR.Interfaces;

namespace TMS.Application.Commands;
public record UpdateEntityCommand<TEntity,TEntityDTO>(int Id, TEntityDTO Entity) : 
    IRequest<DbRequest> 
    where TEntity : Entity
    where TEntityDTO : IDTO;
public class UpdateEntityCommandHandler<TEntity,TEntityDTO>(
    IEntityCommiter entityCommiter,
    IMapper mapper)
    : IRequestHandler<UpdateEntityCommand<TEntity,TEntityDTO>, DbRequest>
    where TEntity : Entity
    where TEntityDTO : IDTO
{
    public async Task<DbRequest> Handle(UpdateEntityCommand<TEntity, TEntityDTO> request, CancellationToken cancellationToken)
    {
        var getRepo = entityCommiter.GetRepository<TEntity>();

        var getEntity = await getRepo.GetAsync(x => x.Id == request.Id);
        if (!getEntity.IsSuccess || getEntity.Data is null)
        {
            return DbRequest.Failure(getEntity?.Message ?? $"Entity of type {typeof(TEntity)} with Id:{request.Id} not found");
        }

        var existingEntity = getEntity.Data;
        mapper.Map(request.Entity, existingEntity);

        var requestUpdate = await getRepo.UpdateAsync(existingEntity);
        if (requestUpdate.IsSuccess)
        {
            await entityCommiter.CommitAsync(cancellationToken);
        }

        return requestUpdate;
    }
}
