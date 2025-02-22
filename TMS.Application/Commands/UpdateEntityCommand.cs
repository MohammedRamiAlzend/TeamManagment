using TMS.Core.MediatR.Interfaces;

namespace TMS.Application.Commands;
public record UpdateEntityCommand<TEntity>(TEntity Entity) : IRequest<DbRequest> where TEntity :  Entity;
public class UpdateEntityCommandHandler<TEntity>(IEntityCommiter entityCommiter)
    : IRequestHandler<UpdateEntityCommand<TEntity>, DbRequest>
    where TEntity : Entity
{
    public async Task<DbRequest> Handle(UpdateEntityCommand<TEntity> request, CancellationToken cancellationToken)
    {
        var requestUpdate = await entityCommiter.GetRepository<TEntity>().UpdateAsync(request.Entity);
        if (requestUpdate.IsSuccess)  await entityCommiter.CommitAsync(cancellationToken);
        return requestUpdate;
    }
}
