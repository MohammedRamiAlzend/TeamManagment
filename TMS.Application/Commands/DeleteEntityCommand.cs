using TMS.Core.MediatR.Interfaces;

namespace TMS.Application.Commands;
public record DeleteEntityCommand<TEntity>(Expression<Func<TEntity, bool>> Filter) : IRequest<DbRequest> where TEntity : Entity;
public class DeleteEntityCommandHandler<TEntity>(IEntityCommiter entityCommiter)
    : IRequestHandler<DeleteEntityCommand<TEntity>, DbRequest>
    where TEntity :  Entity
{
    public async Task<DbRequest> Handle(DeleteEntityCommand<TEntity> request, CancellationToken cancellationToken)
    {
        var requestDelete = await entityCommiter.GetRepository<TEntity>().RemoveAsync(request.Filter);
        if (requestDelete.IsSuccess) await entityCommiter.CommitAsync(cancellationToken);
        return requestDelete;
    }
}
