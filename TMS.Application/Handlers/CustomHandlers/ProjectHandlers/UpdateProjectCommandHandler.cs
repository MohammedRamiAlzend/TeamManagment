using TMS.Contract.CQRS.Commands.CustomCommands.ProjectCommands;
using TMS.Contract.CQRS.Commands.CustomCommands.ProjectCommands.Dtos;
using TMS.Contract.Entities.Enums;
using TMS.Application.Services.Interfaces.ProjectInterfaces;

namespace TMS.Application.Handlers.CustomHandlers.ProjectHandlers;

public class UpdateProjectCommandHandler : IRequestHandler<UpdateProjectCommand, ApiResponse<UpdateProjectDto>>
{
    private readonly IProjectService _projectService;
    private readonly IEntityCommiter _commiter;

    public UpdateProjectCommandHandler(IProjectService projectService, IEntityCommiter commiter)
    {
        _projectService = projectService;
        _commiter = commiter;
    }

    public async Task<ApiResponse<UpdateProjectDto>> Handle(UpdateProjectCommand request, CancellationToken cancellationToken)
    {
        var projectToUpdate = await _commiter.Projects.GetAsync(p => p.Id == request.Id,
            include: i => i.Include(x => x.TeamMembers)
                            .Include(x=>x.Department)
                            .Include(x=>x.Tasks));
        if (projectToUpdate.IsSuccess is false || projectToUpdate.Data is null)
            return ApiResponse<UpdateProjectDto>.Failure(HttpStatusCode.NotFound, "Project not found.");
        if (request.Project.Name is not null)
        {
            projectToUpdate.Data.Name = request.Project.Name;
        }
        if (request.Project.Description is not null)
        {
            projectToUpdate.Data.Description = request.Project.Description;
        }
        if (request.Project.ProjectStatus is not null)
        {
            projectToUpdate.Data.Status = request.Project.ProjectStatus.ToString()!;
        }
        if (request.Project.DepartmentId is not null)
        {
            projectToUpdate.Data.DepartmentId = request.Project.DepartmentId.Value;
        }
        if (request.Project.EnrolledMembersIds is not null)
        {
            var members = await _projectService.GetEnrolledMembers(request.Project.EnrolledMembersIds);
            if (members == null || members.Count != request.Project.EnrolledMembersIds.Count)
                return ApiResponse<UpdateProjectDto>.Failure(HttpStatusCode.BadRequest, "Invalid team member(s) specified.");
            projectToUpdate.Data.TeamMembers = members;
        }
        if (request.Project.GuidTasks is not null)
        {
            var tasks = await _projectService.GetTasksForProject(request.Project.GuidTasks.ToList());
            if (tasks == null || tasks.Count != request.Project.GuidTasks.Count)
                return ApiResponse<UpdateProjectDto>.Failure(HttpStatusCode.BadRequest, "Invalid task(s) specified.");
            projectToUpdate.Data.Tasks = tasks;
        }
        var updateResult = await _commiter.Projects.UpdateAsync(projectToUpdate.Data);
        var commitResult = await _commiter.CommitAsync(cancellationToken);

        return updateResult.IsSuccess is false || commitResult == 0
            ? ApiResponse<UpdateProjectDto>.Failure(HttpStatusCode.BadRequest, updateResult.Message ?? "")
            : ApiResponse<UpdateProjectDto>.Success(request.Project);
    }
} 