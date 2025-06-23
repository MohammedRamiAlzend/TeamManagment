using TMS.Contract.CQRS.Commands.CustomCommands.WorkTaskCommands.Dtos;

namespace TMS.Contract.CQRS.Commands.CustomCommands.WorkTaskCommands;

public record UpdateWorkTaskCommand(UpdateTaskDto Dto,Guid Id):IRequest<ApiResponse>;

