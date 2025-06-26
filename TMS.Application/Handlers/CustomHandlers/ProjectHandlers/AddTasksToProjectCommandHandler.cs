using TMS.Contract.CQRS.Commands.CustomCommands.ProjectCommands;
using TMS.Contract.CQRS.Commands.CustomCommands.ProjectCommands.Dtos;
using TMS.Contract.Entities.Enums;
using TMS.Application.Services;

namespace TMS.Application.Handlers.CustomHandlers.ProjectHandlers;

public class AddTasksToProjectCommandHandler : IRequestHandler<AddTasksToProjectCommand, ApiResponse>
{
    private readonly IAddTasksToProjectValidator _validator;
    private readonly IAddTasksToProjectService _service;
    private readonly IEntityCommiter _commiter;

    public AddTasksToProjectCommandHandler(IAddTasksToProjectValidator validator, IAddTasksToProjectService service, IEntityCommiter commiter)
    {
        _validator = validator;
        _service = service;
        _commiter = commiter;
    }

    public async Task<ApiResponse> Handle(AddTasksToProjectCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.Validate(request.dto.GuidTasks);
        if (!validationResult.IsSuccess)
            return ApiResponse.Failure(HttpStatusCode.BadRequest, validationResult.Message!);
        var getProject = await _commiter.Projects.GetAsync(x => x.Id == request.dto.ProjectId, include: i => i.Include(x => x.Tasks));
        ICollection<WorkTask> getTasksForProject = await _service.GetTasksForProject(request.dto.GuidTasks);
        if (getProject.IsSuccess is false || getProject.Data is null)
        {
            return ApiResponse.Failure(HttpStatusCode.BadRequest, getProject.Message ?? $"no project with {request.dto.ProjectId} was found");
        }
        var project = getProject.Data;
        foreach (var workTask in getTasksForProject)
        {
            project.Tasks.Add(workTask);
        }
        var updateAsync = await _commiter.Projects.UpdateAsync(project);
        var commitResult = await _commiter.CommitAsync(cancellationToken);
        return updateAsync.IsSuccess is false || commitResult == 0
            ? ApiResponse.Failure(HttpStatusCode.BadRequest, updateAsync.Message!)
            : ApiResponse.Success(HttpStatusCode.OK, "Task/tasks has been added successfully");
    }
}