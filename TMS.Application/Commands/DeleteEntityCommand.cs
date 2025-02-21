namespace TMS.Application.Commands;
public record DeleteEntityCommand<TEntity>(TEntity Entity) : IRequest<DbRequest> where TEntity : class, IHasId;
public class DeleteEntityCommandHandler<TEntity>(IEntityCommiter entityCommiter)
    : IRequestHandler<DeleteEntityCommand<TEntity>, DbRequest>
    where TEntity : class, IHasId
{
    public async Task<DbRequest> Handle(DeleteEntityCommand<TEntity> request, CancellationToken cancellationToken)
    {
        var requestDelete = await entityCommiter.GetRepository<TEntity>().RemoveAsync(request.Entity);
        if (requestDelete.IsSuccess) await entityCommiter.CommitAsync(cancellationToken);
        return requestDelete;
    }
}
