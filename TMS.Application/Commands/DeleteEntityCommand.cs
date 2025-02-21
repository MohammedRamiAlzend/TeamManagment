using TMS.Core.MediatR.Interfaces;

namespace TMS.Application.Commands;
public record DeleteEntityCommand<TEntity>(Expression<Func<TEntity, bool>> Filter) : IRequest<DbRequest> where TEntity : class, IHasId;
public class DeleteEntityCommandHandler<TEntity>(IEntityCommiter entityCommiter)
    : IRequestHandler<DeleteEntityCommand<TEntity>, DbRequest>
    where TEntity : class, IHasId
{
    public async Task<DbRequest> Handle(DeleteEntityCommand<TEntity> request, CancellationToken cancellationToken)
    {
        var requestDelete = await entityCommiter.GetRepository<TEntity>().RemoveAsync(request.Filter);
        if (requestDelete.IsSuccess) await entityCommiter.CommitAsync(cancellationToken);
        return requestDelete;
    }
}
