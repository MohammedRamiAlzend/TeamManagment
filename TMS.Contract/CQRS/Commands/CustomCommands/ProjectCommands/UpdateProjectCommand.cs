namespace TMS.Contract.CQRS.Commands.CustomCommands.ProjectCommands;

using Dtos;

public record UpdateProjectCommand(int Id,UpdateProjectDto Project) : IRequest<ApiResponse<UpdateProjectDto>>; 