using TMS.Contract.CQRS.Commands.CustomCommands.WorkTaskCommands.Dtos;

namespace TMS.Contract.CQRS.Commands.CustomCommands.WorkTaskCommands;

public record SubmitTaskCommand(SubmitTaskRequestDto Request , Guid TaskGuid):IRequest<ApiResponse<List<SubmitTaskResponseDto>>>;