namespace TMS.Contract.CQRS.Commands.GenericCommands;

public record AddEntityCommand<TEntityDto>(TEntityDto EntityDto)
    : IRequest<ApiResponse<TEntityDto>> where TEntityDto : IDto;