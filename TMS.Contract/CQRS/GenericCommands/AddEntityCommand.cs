namespace TMS.Contract.CQRS.GenericCommands;

public record AddEntityCommand<TEntityDto>(TEntityDto EntityDto)
    : IRequest<ApiResponse<TEntityDto>> where TEntityDto : IDto;