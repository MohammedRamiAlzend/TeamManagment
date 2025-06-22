namespace TMS.Contract.CQRS.Commands.GenericCommands;

public record DeleteEntityCommand<TEntity>(Expression<Func<TEntity, bool>> Filter)
    : IRequest<ApiResponse> where TEntity : Entity;