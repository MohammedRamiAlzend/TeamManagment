using System.Net;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using TMS.Contract.CQRS.Commands.CustomCommands.WorkTaskCommands;
using TMS.Contract.CQRS.Commands.CustomCommands.WorkTaskCommands.Dtos;
using TMS.Contract.CQRS.Commands.GenericCommands;
using TMS.Contract.CQRS.Queries.CustomQueries.TaskQuries;
using TMS.Contract.CQRS.Queries.CustomQueries.TaskQuries.Dtos;
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

    [HttpPost(TasksEndPoint.SubmitTask)]
    [HasPermission(TaskManagement.SubmitTask)]
    public async Task<ActionResult<ApiResponse<List<SubmitTaskResponseDto>>>> SubmitTaskAsync(
        [FromRoute] Guid taskId,
        [FromForm] SubmitTaskRequestDto request,
        CancellationToken token)
    {
        return await sender.Send(new SubmitTaskCommand(request,taskId), token);
    }

    [HttpGet(TasksEndPoint.GetSubmissionFiles)]
    [HasPermission(TaskManagement.Get)]
    public async Task<ActionResult<ApiResponse<List<SubmissionFileDto>>>> GetSubmissionFilesAsync(
        [FromRoute] Guid taskId,
        CancellationToken token)
    {
        return await sender.Send(new GetTaskSubmissionFilesQuery(taskId), token);
    }

    [HttpGet(TasksEndPoint.DownloadSubmissionFile)]
    [HasPermission(TaskManagement.Get)]
    public async Task<IActionResult> DownloadSubmissionFileAsync(
        [FromRoute] Guid taskId,
        [FromRoute] int fileId,
        CancellationToken token)
    {
        var result = await sender.Send(new GetSubmissionFileQuery(taskId, fileId), token);

        if (!result.IsSuccess || result.Data == null)
        {
            return StatusCode((int)(result.Code ?? HttpStatusCode.BadRequest), result);
        }

        return File(result.Data.FileContents, result.Data.ContentType, result.Data.FileName);
    }
    
    [HttpGet(TasksEndPoint.DownloadAllFiles)]
    [HasPermission(TaskManagement.Get)]
    public async Task<IActionResult> DownloadAllSubmissionsFilesAsync(
        [FromRoute] Guid taskId,
        CancellationToken token)
    {
        var result = await sender.Send(new GetAllSubmissionsFilesQuery(taskId), token);

        if (!result.IsSuccess || result.Data == null)
        {
            return StatusCode((int)(result.Code ?? HttpStatusCode.BadRequest), result);
        }

        return File(result.Data.ZipFileContents, result.Data.ContentType, result.Data.ZipFileName);
    }
}