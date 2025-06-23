namespace TMS.Contract.CQRS.Commands.CustomCommands.ProjectCommands;

using Dtos;

public record UpdateProjectCommand(UpdateProjectDto Project) : IRequest<ApiResponse<UpdateProjectDto>>; 