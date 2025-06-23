using TMS.Contract.CQRS.Commands.CustomCommands.WorkTaskCommands;
using TMS.Contract.CQRS.Commands.CustomCommands.WorkTaskCommands.Dtos;
using TMS.Contract.CQRS.Commands.GenericCommands;
using TMS.Contract.CQRS.Queries.CustomQueries.TaskQuries;
using TMS.Contract.CQRS.Queries.GenericQueries;

namespace TMS.Server.Controllers;

[ApiController]
[Route($"{ApiBase}/[controller]")]
[Authorize]
public class TaskController(ISender sender) : ControllerBase
{
    [HttpGet(TasksEndPoint.GetAll)]
    [HasPermission(TaskManagement.Get)]
    public async Task<ActionResult<ApiResponse<List<GetTaskResponse>>>> GetAllTasksAsync(CancellationToken token)
    {
        return await sender.Send(new GetAllEntityQuery<WorkTask, GetTaskResponse>(
            Include:QueryIncludeHelper.IncludeTaskRelations()
        ), token);
    }
    

    [HttpGet(TasksEndPoint.Get)]
    [HasPermission(TaskManagement.Get)]
    public async Task<ActionResult<ApiResponse<GetTaskResponse>>> GetTaskByIdAsync(
        [FromRoute] Guid taskId,
        CancellationToken token)
    {
        return await sender.Send(new GetEntityQuery<WorkTask, GetTaskResponse>(
            Filter: x => x.TaskUniqueIdentifier == taskId,
            Include:QueryIncludeHelper.IncludeTaskRelations()
        ), token);
    }
    [HttpGet(TasksEndPoint.GetAllPaginated)]
    [HasPermission(TaskManagement.Get)]
    public async Task<ActionResult<PaginatedApiResponse<GetTaskResponse>>> GetAllTasksPaginatedAsync(
        [FromQuery] int pageNumber,
        [FromQuery] int pageSize,
        CancellationToken token)
    {
        if (pageNumber <= 0 || pageSize <= 0) return BadRequest("Invalid pagination parameters.");

        return await sender.Send(
            new GetAllPaginatedEntityQuery<WorkTask, GetTaskResponse>(
                PageNumber: pageNumber, PageSize: pageSize,
                Include:QueryIncludeHelper.IncludeTaskRelations()
            ), token);
    }
    [HttpPost(TasksEndPoint.Create)]
    [HasPermission(TaskManagement.Add)]
    public async Task<ActionResult<ApiResponse>> AddTaskAsync(
        [FromBody] AddTaskDto task,
        CancellationToken token)
    {
        return await sender.Send(new AddEntityCommand<AddTaskDto>(task), token);
    }
    
    [HttpPut(TasksEndPoint.Update)]
    [HasPermission(TaskManagement.Update)]
    public async Task<ActionResult<ApiResponse>> UpdateProjectAsync(
        [FromRoute] Guid taskId,
        [FromBody] UpdateTaskDto task,
        CancellationToken token)
    {
        return await sender.Send(new UpdateWorkTaskCommand(task,taskId), token);
    }
    
    [HttpDelete(TasksEndPoint.Delete)]
    [HasPermission(TaskManagement.Delete)]
    public async Task<ActionResult<ApiResponse>> DeleteProjectAsync(Guid taskId,
        CancellationToken token)
    {
        return await sender.Send(new DeleteEntityCommand<WorkTask>(x=>x.TaskUniqueIdentifier==taskId), token);
    }
}