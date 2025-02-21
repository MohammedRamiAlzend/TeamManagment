using TMS.Core.MediatR.Interfaces;

namespace TMS.Application.Commands;
public record AddEntityCommand<TEntity>(TEntity Entity) : IRequest<DbRequest> where TEntity : class, IHasId;
public class AddEntityCommandHandler<TEntity>(IEntityCommiter entityCommiter)
    : IRequestHandler<AddEntityCommand<TEntity>, DbRequest>
    where TEntity : class, IHasId
{
    public async Task<DbRequest> Handle(AddEntityCommand<TEntity> request, CancellationToken cancellationToken)
    {
        var requestAdd = await entityCommiter.GetRepository<TEntity>().AddAsync(request.Entity);
        if (requestAdd.IsSuccess) await entityCommiter.CommitAsync(cancellationToken);
        return requestAdd;
    }
}
