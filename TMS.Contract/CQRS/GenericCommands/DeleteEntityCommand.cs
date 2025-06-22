namespace TMS.Contract.CQRS.GenericCommands;

public record DeleteEntityCommand<TEntity>(Expression<Func<TEntity, bool>> Filter)
    : IRequest<ApiResponse> where TEntity : Entity;