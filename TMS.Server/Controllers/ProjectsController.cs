using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using TMS.Application.Handlers.CustomHandlers.ProjectHandlers;
using TMS.Contract.CQRS.Commands.CustomCommands.ProjectCommands;
using TMS.Contract.CQRS.Commands.CustomCommands.ProjectCommands.Dtos;
using TMS.Contract.CQRS.Commands.GenericCommands;
using TMS.Contract.CQRS.Queries.CustomQueries.ProjectQuries;
using TMS.Contract.CQRS.Queries.GenericQueries;
using Microsoft.Extensions.Logging;

namespace TMS.Server.Controllers;
/// <summary>
/// Controller for managing projects.
/// </summary>
[ApiController]
[Route($"{ApiBase}/[controller]")]
[Authorize]
public class ProjectsController : ControllerBase
{
    private readonly ISender _sender;
    private readonly ILogger<ProjectsController> _logger;

    public ProjectsController(ISender sender, ILogger<ProjectsController> logger)
    {
        _sender = sender;
        _logger = logger;
    }

    /// <summary>
    /// Gets all projects.
    /// </summary>
    [HttpGet(ProjectsEndPoint.GetAll)]
    [HasPermission(ProjectManagement.Get)]
    [ProducesResponseType(typeof(ApiResponse<List<GetProjectResponse>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<List<GetProjectResponse>>>> GetAllProjectsAsync(CancellationToken token)
    {
        try
        {
            var result = await _sender.Send(new GetAllEntityQuery<Project, GetProjectResponse>(
                Include: QueryIncludeHelper.IncludeProjectRelations()
            ), token);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all projects");
            return StatusCode(500, "An error occurred while retrieving projects.");
        }
    }

    /// <summary>
    /// Gets all projects with pagination.
    /// </summary>
    [HttpGet(ProjectsEndPoint.GetAllPaginated)]
    [HasPermission(ProjectManagement.Get)]
    [ProducesResponseType(typeof(PaginatedApiResponse<GetProjectResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PaginatedApiResponse<GetProjectResponse>>> GetAllProjectsPaginatedAsync(
        [FromQuery] int pageSize,
        [FromQuery] int pageNumber,
        CancellationToken token)
    {
        try
        {
            var result = await _sender.Send(new GetAllPaginatedEntityQuery<Project, GetProjectResponse>(
                PageSize: pageSize,
                PageNumber: pageNumber,
                Include: QueryIncludeHelper.IncludeProjectRelations()
            ), token);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting paginated projects");
            return StatusCode(500, "An error occurred while retrieving paginated projects.");
        }
    }

    /// <summary>
    /// Gets a project by ID.
    /// </summary>
    [HttpGet(ProjectsEndPoint.Get)]
    [HasPermission(ProjectManagement.Get)]
    [ProducesResponseType(typeof(ApiResponse<GetProjectResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<GetProjectResponse>>> GetProjectAsync(int projectId, CancellationToken token)
    {
        try
        {
            var result = await _sender.Send(new GetEntityQuery<Project, GetProjectResponse>(
                Filter: x => x.Id == projectId,
                Include: QueryIncludeHelper.IncludeProjectRelations()
            ), token);
            if (result == null)
            {
                _logger.LogWarning("Project not found: {ProjectId}", projectId);
                return NotFound("Project not found.");
            }
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting project by ID");
            return StatusCode(500, "An error occurred while retrieving the project.");
        }
    }

    /// <summary>
    /// Adds a new project.
    /// </summary>
    [HttpPost(ProjectsEndPoint.Create)]
    [HasPermission(ProjectManagement.Add)]
    [ProducesResponseType(typeof(ApiResponse<AddProjectDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<AddProjectDto>>> AddProjectAsync([FromBody] AddProjectDto project,
        CancellationToken token)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Invalid model state for AddProjectAsync");
            return BadRequest(ModelState);
        }
        try
        {
            var result = await _sender.Send(new AddProjectCommand(project), token);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding project");
            return StatusCode(500, "An error occurred while adding the project.");
        }
    }

    /// <summary>
    /// Adds tasks to a project.
    /// </summary>
    [HttpPost(ProjectsEndPoint.AddTasks)]
    [HasPermission(ProjectManagement.Add)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse>> AddTasksToProjectAsync(
        [FromForm] AddTasksToProjectDto project,
        CancellationToken token)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Invalid model state for AddTasksToProjectAsync");
            return BadRequest(ModelState);
        }
        try
        {
            var result = await _sender.Send(new AddTasksToProjectCommand(project), token);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding tasks to project");
            return StatusCode(500, "An error occurred while adding tasks to the project.");
        }
    }

    /// <summary>
    /// Updates a project.
    /// </summary>
    [HttpPut(ProjectsEndPoint.Update)]
    [HasPermission(ProjectManagement.Update)]
    [ProducesResponseType(typeof(ApiResponse<UpdateProjectDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<UpdateProjectDto>>> UpdateProjectAsync(
        [FromRoute] int projectId,
        [FromBody] UpdateProjectDto project,
        CancellationToken token)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Invalid model state for UpdateProjectAsync");
            return BadRequest(ModelState);
        }
        try
        {
            var result = await _sender.Send(new UpdateProjectCommand(projectId, project), token);
            if (result == null)
            {
                _logger.LogWarning("Project not found for update: {ProjectId}", projectId);
                return NotFound("Project not found.");
            }
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating project");
            return StatusCode(500, "An error occurred while updating the project.");
        }
    }

    /// <summary>
    /// Deletes a project.
    /// </summary>
    [HttpDelete(ProjectsEndPoint.Delete)]
    [HasPermission(ProjectManagement.Delete)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteProjectAsync(int projectId,
        CancellationToken token)
    {
        try
        {
            var result = await _sender.Send(new DeleteProjectCommand(projectId), token);
            if (result == null)
            {
                _logger.LogWarning("Project not found for delete: {ProjectId}", projectId);
                return NotFound("Project not found.");
            }
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting project");
            return StatusCode(500, "An error occurred while deleting the project.");
        }
    }
}