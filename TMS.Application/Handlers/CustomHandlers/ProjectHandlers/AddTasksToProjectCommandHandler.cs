using TMS.Contract.CQRS.Commands.CustomCommands.ProjectCommands;
using TMS.Contract.CQRS.Commands.CustomCommands.ProjectCommands.Dtos;
using TMS.Contract.Entities.Enums;

namespace TMS.Application.Handlers.CustomHandlers.ProjectHandlers;

public class AddTasksToProjectCommandHandler(IEntityCommiter commiter):IRequestHandler<AddTasksToProjectCommand,ApiResponse>
{
     public async Task<ApiResponse> Handle(AddTasksToProjectCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await ValidateDto(request.dto.GuidTasks);
        if(validationResult.IsSuccess is false)
            return ApiResponse.Failure(HttpStatusCode.BadRequest, validationResult.Message!);
        var getProject = await commiter.Projects.GetAsync(x => x.Id == request.dto.ProjectId,include:i=>i.Include(x=>x.Tasks));
        ICollection<WorkTask> getTasksForProject = await GetTasksForProject(request.dto.GuidTasks);
        if (getProject.IsSuccess is false || getProject.Data is null)
        {
            return ApiResponse.Failure(HttpStatusCode.BadRequest, getProject.Message??$"no project with {request.dto.ProjectId} was found");
        }
        var project = getProject.Data;
        foreach (var workTask in getTasksForProject)
        {
            project.Tasks.Add(workTask);
        }
        var updateAsync =await commiter.Projects.UpdateAsync(project);
        var commitResult = await commiter.CommitAsync(cancellationToken);
        return updateAsync.IsSuccess is false  || commitResult == 0
            ? ApiResponse.Failure(HttpStatusCode.BadRequest, updateAsync.Message!) 
            : ApiResponse.Success(HttpStatusCode.OK,"Task/tasks has been added successfully");
    }
    private async Task<List<WorkTask>> GetTasksForProject(List<Guid> tasksIds)
    {
        var tasks = new List<WorkTask>();
        foreach (var id in tasksIds)
        {
            tasks.Add((await commiter.Tasks.GetAsync(x => x.TaskUniqueIdentifier == id)).Data!);
        }

        return tasks;
    }
    private async Task<DbRequest> ValidateDto(List<Guid> taskIds)
    {
        var errorMessage = "";
        foreach (var taskId in taskIds)
        {
            if(await commiter.Tasks.AnyAsync(x => x.TaskUniqueIdentifier == taskId) is false)
                errorMessage += $"Task with id {taskId} does not exist\n";       
        }        
        return errorMessage.Length > 0 ? DbRequest.Failure(errorMessage) : DbRequest.Success();
    }
}