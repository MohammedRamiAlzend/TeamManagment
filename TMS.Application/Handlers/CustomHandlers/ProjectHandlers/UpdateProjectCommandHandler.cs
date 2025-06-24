using TMS.Contract.CQRS.Commands.CustomCommands.ProjectCommands;
using TMS.Contract.CQRS.Commands.CustomCommands.ProjectCommands.Dtos;
using TMS.Contract.Entities.Enums;

namespace TMS.Application.Handlers.CustomHandlers.ProjectHandlers;

public class UpdateProjectCommandHandler(IEntityCommiter commiter) : IRequestHandler<UpdateProjectCommand, ApiResponse<UpdateProjectDto>>
{
    public async Task<ApiResponse<UpdateProjectDto>> Handle(UpdateProjectCommand request, CancellationToken cancellationToken)
    {

        var projectToUpdate = await commiter.Projects.GetAsync(p => p.Id == request.Id,
            include:i=> i.Include(x=>x.TeamMembers));
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
            projectToUpdate.Data.TeamMembers = await GetEnrolledMembers(request.Project.EnrolledMembersIds);
        }
        if (request.Project.Tasks is not null)
        {
            projectToUpdate.Data.Tasks = await GetTasksForProject(request.Project.Tasks);
        }
        var updateResult = await commiter.Projects.UpdateAsync(projectToUpdate.Data);
        var commitResult = await commiter.CommitAsync(cancellationToken);

        return updateResult.IsSuccess is false || commitResult == 0
            ? ApiResponse<UpdateProjectDto>.Failure(HttpStatusCode.BadRequest, updateResult.Message??"")
            : ApiResponse<UpdateProjectDto>.Success(request.Project);
    }
    
    private async Task<List<Employee>> GetEnrolledMembers(List<int> enrolledMembersIds)
    {
        var employees = new List<Employee>();
        foreach (var id in enrolledMembersIds)
        {
            employees.Add((await commiter.Employees.GetAsync(x => x.Id == id)).Data!);
        }
        return employees;
    }
    private async Task<List<WorkTask>> GetTasksForProject(ICollection<Guid> tasksIds)
    {
        var tasks = new List<WorkTask>();
        foreach (var id in tasksIds)
        {
            tasks.Add((await commiter.Tasks.GetAsync(x => x.TaskUniqueIdentifier == id)).Data!);
        }
        return tasks;
    }
} 