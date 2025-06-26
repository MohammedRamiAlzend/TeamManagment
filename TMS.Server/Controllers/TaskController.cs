using System.Net;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using TMS.Contract.CQRS.Commands.CustomCommands.WorkTaskCommands;
using TMS.Contract.CQRS.Commands.CustomCommands.WorkTaskCommands.Dtos;
using TMS.Contract.CQRS.Commands.GenericCommands;
using TMS.Contract.CQRS.Queries.CustomQueries.TaskQuries;
using TMS.Contract.CQRS.Queries.CustomQueries.TaskQuries.Dtos;
using TMS.Contract.CQRS.Queries.GenericQueries;
using Microsoft.Extensions.Logging;
using TMS.Application.Handlers.CustomHandlers.WorkTaskHandlers.CommandHanlders;
using TMS.Application.Handlers.CustomHandlers.WorkTaskHandlers.QueryHandlers;

namespace TMS.Server.Controllers;

/// <summary>
/// Controller for managing tasks.
/// </summary>
[ApiController]
[Route($"{ApiBase}/[controller]")]
[Authorize]
public class TaskController(ISender sender, ILogger<TaskController> logger) : ControllerBase
{

    /// <summary>
    /// Gets all tasks.
    /// </summary>
    [HttpGet(TasksEndPoint.GetAll)]
    [HasPermission(TaskManagement.Get)]
    [ProducesResponseType(typeof(ApiResponse<List<GetTaskResponse>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<List<GetTaskResponse>>>> GetAllTasksAsync(CancellationToken token)
    {
        try
        {
            var result = await sender.Send(new GetAllEntityQuery<WorkTask, GetTaskResponse>(
                Include: QueryIncludeHelper.IncludeTaskRelations()
            ), token);
            return Ok(result);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting all tasks");
            return StatusCode(500, "An error occurred while retrieving tasks.");
        }
    }
    

    /// <summary>
    /// Gets a task by ID.
    /// </summary>
    [HttpGet(TasksEndPoint.Get)]
    [HasPermission(TaskManagement.Get)]
    [ProducesResponseType(typeof(ApiResponse<GetTaskResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<GetTaskResponse>>> GetTaskByIdAsync(
        [FromRoute] Guid taskGuidId,
        CancellationToken token)
    {
        try
        {
            var result = await sender.Send(new GetEntityQuery<WorkTask, GetTaskResponse>(
                Filter: x => x.TaskUniqueIdentifier == taskGuidId,
                Include:QueryIncludeHelper.IncludeTaskRelations()
            ), token);
            if (result == null)
            {
                logger.LogWarning("Task not found: {TaskId}", taskGuidId);
                return NotFound("Task not found.");
            }
            return Ok(result);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting task by ID");
            return StatusCode(500, "An error occurred while retrieving the task.");
        }
    }
    /// <summary>
    /// Gets all tasks with pagination.
    /// </summary>
    [HttpGet(TasksEndPoint.GetAllPaginated)]
    [HasPermission(TaskManagement.Get)]
    [ProducesResponseType(typeof(PaginatedApiResponse<GetTaskResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PaginatedApiResponse<GetTaskResponse>>> GetAllTasksPaginatedAsync(
        [FromQuery] int pageNumber,
        [FromQuery] int pageSize,
        CancellationToken token)
    {
        if (pageNumber <= 0 || pageSize <= 0)
        {
            logger.LogWarning("Invalid pagination parameters: pageNumber={PageNumber}, pageSize={PageSize}", pageNumber, pageSize);
            return BadRequest("Invalid pagination parameters.");
        }
        try
        {
            var result = await sender.Send(
                new GetAllPaginatedEntityQuery<WorkTask, GetTaskResponse>(
                    PageNumber: pageNumber, PageSize: pageSize,
                    Include:QueryIncludeHelper.IncludeTaskRelations()
                ), token);
            return Ok(result);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting paginated tasks");
            return StatusCode(500, "An error occurred while retrieving paginated tasks.");
        }
    }
    
    /// <summary>
    /// Adds a new task.
    /// </summary>
    [HttpPost(TasksEndPoint.Create)]
    [HasPermission(TaskManagement.Add)]
    [ProducesResponseType(typeof(ApiResponse<AddTaskResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<AddTaskResponseDto>>> AddTaskAsync(
        [FromBody] AddTaskDto task,
        CancellationToken token)
    {
        if (!ModelState.IsValid)
        {
            logger.LogWarning("Invalid model state for AddTaskAsync");
            return BadRequest(ModelState);
        }
        try
        {
            var result = await sender.Send(new AddTaskCommand(task), token);
            return Ok(result);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error adding task");
            return StatusCode(500, "An error occurred while adding the task.");
        }
    }
    
    /// <summary>
    /// Updates a task.
    /// </summary>
    [HttpPut(TasksEndPoint.Update)]
    [HasPermission(TaskManagement.Update)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse>> UpdateProjectAsync(
        [FromRoute] Guid taskGuidId,
        [FromBody] UpdateTaskDto task,
        CancellationToken token)
    {
        if (!ModelState.IsValid)
        {
            logger.LogWarning("Invalid model state for UpdateProjectAsync");
            return BadRequest(ModelState);
        }
        try
        {
            var result = await sender.Send(new UpdateWorkTaskCommand(task, taskGuidId), token);
            if (result == null)
            {
                logger.LogWarning("Task not found for update: {TaskId}", taskGuidId);
                return NotFound("Task not found.");
            }
            return Ok(result);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error updating task");
            return StatusCode(500, "An error occurred while updating the task.");
        }
    }
    
    /// <summary>
    /// Deletes a task.
    /// </summary>
    [HttpDelete(TasksEndPoint.Delete)]
    [HasPermission(TaskManagement.Delete)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse>> DeleteProjectAsync(
        [FromRoute]Guid taskGuidId,
        CancellationToken token)
    {
        try
        {
            var result = await sender.Send(new DeleteEntityCommand<WorkTask>(x => x.TaskUniqueIdentifier == taskGuidId), token);
            if (result == null)
            {
                logger.LogWarning("Task not found for delete: {TaskId}", taskGuidId);
                return NotFound("Task not found.");
            }
            return Ok(result);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error deleting task");
            return StatusCode(500, "An error occurred while deleting the task.");
        }
    }

    /// <summary>
    /// Submits a task.
    /// </summary>
    [HttpPost(TasksEndPoint.SubmitTask)]
    [HasPermission(TaskManagement.SubmitTask)]
    [ProducesResponseType(typeof(ApiResponse<List<SubmitTaskResponseDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<List<SubmitTaskResponseDto>>>> SubmitTaskAsync(
        [FromRoute] Guid taskGuidId,
        [FromForm] SubmitTaskRequestDto request,
        CancellationToken token)
    {
        if (!ModelState.IsValid)
        {
            logger.LogWarning("Invalid model state for SubmitTaskAsync");
            return BadRequest(ModelState);
        }
        try
        {
            var result = await sender.Send(new SubmitTaskCommand(request, taskGuidId), token);
            return Ok(result);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error submitting task");
            return StatusCode(500, "An error occurred while submitting the task.");
        }
    }

    /// <summary>
    /// Gets submission files for a task.
    /// </summary>
    [HttpGet(TasksEndPoint.GetSubmissionFiles)]
    [HasPermission(TaskManagement.Get)]
    [ProducesResponseType(typeof(ApiResponse<List<SubmissionFileDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<List<SubmissionFileDto>>>> GetSubmissionFilesAsync(
        [FromRoute] Guid taskGuidId,
        CancellationToken token)
    {
        try
        {
            var result = await sender.Send(new GetTaskSubmissionFilesQuery(taskGuidId), token);
            return Ok(result);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting submission files");
            return StatusCode(500, "An error occurred while retrieving submission files.");
        }
    }

    ///// <summary>
    ///// Downloads a submission file for a task.
    ///// </summary>
    //[HttpGet(TasksEndPoint.DownloadSubmissionFile)]
    //[HasPermission(TaskManagement.Get)]
    //[ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    //[ProducesResponseType(StatusCodes.Status404NotFound)]
    //public async Task<IActionResult> DownloadSubmissionFileAsync(
    //    [FromRoute] Guid taskGuidId,
    //    [FromRoute] Guid fileGuidId,
    //    CancellationToken token)
    //{
    //    try
    //    {
    //        var result = await sender.Send(new GetSubmissionFileQuery(taskGuidId, fileGuidId), token);
    //        if (!result.IsSuccess || result.Data == null)
    //        {
    //            logger.LogWarning("Submission file not found: TaskId={TaskId}, FileId={FileId}", taskGuidId, fileGuidId);
    //            return StatusCode((int)(result.Code ?? HttpStatusCode.BadRequest), result);
    //        }
    //        return File(result.Data.FileContents, result.Data.ContentType, result.Data.FileName);
    //    }
    //    catch (Exception ex)
    //    {
    //        logger.LogError(ex, "Error downloading submission file");
    //        return StatusCode(500, "An error occurred while downloading the submission file.");
    //    }
    //}
    
    /// <summary>
    /// Downloads all submission files for a task as a zip.
    /// </summary>
    [HttpGet(TasksEndPoint.DownloadAllFiles)]
    [HasPermission(TaskManagement.Get)]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DownloadAllSubmissionsFilesAsync(
        [FromRoute] Guid taskId,
        CancellationToken token)
    {
        try
        {
            var result = await sender.Send(new GetAllSubmissionsFilesQuery(taskId), token);
            if (!result.IsSuccess || result.Data == null)
            {
                logger.LogWarning("Submission zip not found: TaskId={TaskId}", taskId);
                return StatusCode((int)(result.Code ?? HttpStatusCode.BadRequest), result);
            }
            return File(result.Data.ZipFileContents, result.Data.ContentType, result.Data.ZipFileName);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error downloading all submission files");
            return StatusCode(500, "An error occurred while downloading all submission files.");
        }
    }

    /// <summary>
    /// Gets all submissions for a specific task.
    /// </summary>
    [HttpGet(TaskSubmissionsEndPoint.GetAll)]
    [HasPermission(TaskManagement.Get)]
    [ProducesResponseType(typeof(ApiResponse<List<TaskSubmission>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<List<TaskSubmission>>>> GetTaskSubmissionsAsync(
        [FromRoute] Guid guidTaskId,
        CancellationToken token)
    {
        var result = await sender.Send(new GetTaskSubmissionsQuery(guidTaskId), token);
        return Ok(result);
    }

    /// <summary>
    /// Gets a specific submission by ID.
    /// </summary>
    [HttpGet(TaskSubmissionsEndPoint.GetById)]
    [HasPermission(TaskManagement.Get)]
    [ProducesResponseType(typeof(ApiResponse<TaskSubmission>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<TaskSubmission>>> GetTaskSubmissionAsync(
        [FromRoute] Guid guidTaskId,
        [FromRoute] Guid submissionGuidId,
        CancellationToken token)
    {
        var result = await sender.Send(new GetTaskSubmissionByIdQuery(submissionGuidId), token);
        return result.Data == null ? NotFound(result) : Ok(result);
    }

   

    /// <summary>
    /// Updates a submission.
    /// </summary>
    [HttpPut(TaskSubmissionsEndPoint.Update)]
    [HasPermission(TaskManagement.Update)]
    [ProducesResponseType(typeof(ApiResponse<TaskSubmission>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<TaskSubmission>>> UpdateTaskSubmissionAsync(
        [FromRoute] Guid guidTaskId,
        [FromRoute] Guid submissionGuidId,
        [FromBody] TaskSubmission updatedSubmission,
        CancellationToken token)
    {
        updatedSubmission.WorkTaskId = guidTaskId.GetHashCode();
        var result = await sender.Send(new UpdateTaskSubmissionCommand(submissionGuidId, updatedSubmission), token);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// Deletes a submission.
    /// </summary>
    [HttpDelete(TaskSubmissionsEndPoint.Delete)]
    [HasPermission(TaskManagement.Delete)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse>> DeleteTaskSubmissionAsync(
        [FromRoute] Guid guidTaskId,
        [FromRoute] Guid submissionGuidId,
        CancellationToken token)
    {
        var result = await sender.Send(new DeleteTaskSubmissionCommand(submissionGuidId), token);
        return result.IsSuccess ? Ok(result) : NotFound(result);
    }
}