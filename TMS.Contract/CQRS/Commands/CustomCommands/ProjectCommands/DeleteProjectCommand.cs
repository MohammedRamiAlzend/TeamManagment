namespace TMS.Contract.CQRS.Commands.CustomCommands.ProjectCommands;

public record DeleteProjectCommand(int ProjectId) : IRequest<ApiResponse<bool>>;