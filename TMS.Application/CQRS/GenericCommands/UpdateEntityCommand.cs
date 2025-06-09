namespace TMS.Application.CQRS.GenericCommands;

public record UpdateEntityCommand<TEntity, TEntityDto>(int Id, TEntityDto Entity) : IRequest<ApiResponse<TEntityDto>>
    where TEntity : Entity
    where TEntityDto : IDto;