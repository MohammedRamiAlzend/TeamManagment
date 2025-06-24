using TMS.Contract.CQRS.Commands.CustomCommands.ProjectCommands.Dtos;

namespace TMS.Contract.CQRS.Commands.CustomCommands.ProjectCommands;

public record AddTasksToProjectCommand(AddTasksToProjectDto dto):IRequest<ApiResponse>;