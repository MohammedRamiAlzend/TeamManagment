using TMS.Contract.CQRS.Commands.CustomCommands.WorkTaskCommands.Dtos;

namespace TMS.Contract.CQRS.Commands.CustomCommands.WorkTaskCommands;

public record SubmitTaskCommand(SubmitTaskRequestDto Request ):IRequest<ApiResponse<List<SubmitTaskResponseDto>>>;