using TMS.Contract.CQRS.Commands.CustomCommands.DepartmentCommands;
using TMS.Contract.CQRS.Queries.CustomQueries.DepartmentQuries;
using TMS.Contract.CQRS.Queries.CustomQueries.TaskQuries;
using TMS.Contract.CQRS.Queries.GenericQueries;

namespace TMS.Server.Controllers;

[ApiController]
[Route($"{ApiBase}[controller]")]
[Authorize]
public class TaskController(ISender sender) : ControllerBase
{
    [HttpGet(TasksEndPoint.GetAll)]
    [HasPermission(DepartmentManagement.Get)]
    public async Task<ActionResult<ApiResponse<List<GetTaskResponse>>>> GetAllTasksAsync(CancellationToken token)
    {
        return await sender.Send(new GetAllEntityQuery<WorkTask, GetTaskResponse>(
            Include:QueryIncludeHelper.IncludeTaskRelations()
        ), token);
    }
    

    [HttpGet(TasksEndPoint.Get)]
    [HasPermission(DepartmentManagement.Get)]
    public async Task<ActionResult<ApiResponse<GetTaskResponse>>> GetTaskByIdAsync(
        [FromRoute] int taskId,
        CancellationToken token)
    {
        if (taskId <= 0) return BadRequest("Invalid Task ID.");
        return await sender.Send(new GetEntityQuery<WorkTask, GetTaskResponse>(
            Filter: x => x.Id == taskId,
            Include:QueryIncludeHelper.IncludeTaskRelations()
        ), token);
    }
    [HttpGet(TasksEndPoint.GetAllPaginated)]
    [HasPermission(EmployeeManagement.Get)]
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

}