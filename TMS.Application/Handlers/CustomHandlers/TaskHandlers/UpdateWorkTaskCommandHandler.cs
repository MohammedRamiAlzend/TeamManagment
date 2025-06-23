using Microsoft.EntityFrameworkCore.Query;
using TMS.Contract.CQRS.Commands.CustomCommands.WorkTaskCommands;
using System.Text;
using TMS.Contract.CQRS.Commands.CustomCommands.WorkTaskCommands.Dtos;

namespace TMS.Application.Handlers.CustomHandlers.TaskHandlers;

public class UpdateTaskCommandHandler(IEntityCommiter commiter) : IRequestHandler<UpdateWorkTaskCommand, ApiResponse>
{
    public async Task<ApiResponse> Handle(UpdateWorkTaskCommand request, CancellationToken cancellationToken)
    {
        if (request?.Dto == null)
            return ApiResponse.Failure(HttpStatusCode.BadRequest, "Invalid request.");

        var taskToUpdate = await commiter.Tasks.GetAsync(
            x => x.TaskUniqueIdentifier == request.Id, 
            include: IncludeTaskRelations());

        if (taskToUpdate.IsSuccess is false || taskToUpdate.Data == null)
            return ApiResponse.Failure(HttpStatusCode.NotFound, "Task not found.");

        var relationshipUpdateResult = await UpdateRelationships(request.Dto, taskToUpdate.Data);
        if (!relationshipUpdateResult.IsSuccess)
            return ApiResponse.Failure(HttpStatusCode.NotFound, relationshipUpdateResult.Message ?? "Failed to update relationships.");

        UpdateTaskProperties(request.Dto, taskToUpdate.Data);

        var updateResult = await commiter.Tasks.UpdateAsync(taskToUpdate.Data);
        var commitResult = await commiter.CommitAsync(cancellationToken);

        return updateResult.IsSuccess is false || commitResult == 0
            ? ApiResponse.Failure(HttpStatusCode.InternalServerError, updateResult.Message ?? "Failed to update the task.")
            : ApiResponse.Success();
    }

    private void UpdateTaskProperties(UpdateTaskDto dto, WorkTask task)
    {
        task.Title = dto.Title ?? task.Title;
        task.Accepted = dto.Accepted ?? task.Accepted;
        task.AssignedToEmployeeId = dto.AssignedToEmployeeId ?? task.AssignedToEmployeeId;
        task.CreatedByEmployeeId = dto.CreatedByEmployeeId ?? task.CreatedByEmployeeId;
        task.DeadLine = dto.DeadLine ?? task.DeadLine;
        task.Description = dto.Description ?? task.Description;
        task.EndDate = dto.EndDate ?? task.EndDate;
        task.PointsValue = dto.PointsValue ?? task.PointsValue;
        task.Priority = dto.Priority ?? task.Priority;
        task.StartDate = dto.StartDate ?? task.StartDate;
        task.Status = dto.Status ?? task.Status;
    }

    private async Task<ApiResponse> UpdateRelationships(UpdateTaskDto dto, WorkTask task)
    {
        if (dto.ProjectIds is { Count: > 0 })
        {
            var dbRequest = await GetProjects(dto.ProjectIds.ToList());
            if (dbRequest.IsSuccess)
            {
                task.Projects = dbRequest.Data ?? new List<Project>();
            }
            else
            {
                return ApiResponse.Failure(HttpStatusCode.NotFound, dbRequest.Message ?? "");
            }
        }
        return ApiResponse.Success();
    }

    private async Task<DbRequest<List<Project>>> GetProjects(List<int> projectsIds)
    {
        var projects = new List<Project>();
        var errors = new StringBuilder();

        foreach (var id in projectsIds)
        {
            var request = await commiter.Projects.GetAsync(x => x.Id == id);
            if (request is { IsSuccess: true, Data: not null })
                projects.Add(request.Data);
            else
                errors.AppendLine($"Project with ID {id} was not found.");
        }

        return errors.Length == 0
            ? DbRequest<List<Project>>.Success(projects)
            : DbRequest<List<Project>>.Failure(errors.ToString());
    }

    private static Func<IQueryable<WorkTask>, IIncludableQueryable<WorkTask, object>>? IncludeTaskRelations()
    {
        return query => query
            .Include(e => e.CreatedBy).ThenInclude(e => e.User).ThenInclude(e => e.Roles)
            .Include(e => e.AssignedTo).ThenInclude(e => e.User).ThenInclude(e => e.Roles)
            .Include(e => e.Projects).ThenInclude(x => x.Department);
    }
}