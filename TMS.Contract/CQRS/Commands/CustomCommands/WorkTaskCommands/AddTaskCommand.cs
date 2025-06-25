using TMS.Contract.CQRS.Commands.CustomCommands.WorkTaskCommands.Dtos;

namespace TMS.Contract.CQRS.Commands.CustomCommands.WorkTaskCommands;

public record AddTaskCommand(AddTaskDto Dto) : IRequest<ApiResponse<AddTaskResponseDto>>;
