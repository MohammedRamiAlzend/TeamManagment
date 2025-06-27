using TMS.Contract.CQRS.Commands.CustomCommands.DepartmentCommands;
using TMS.Contract.CQRS.Commands.CustomCommands.DepartmentCommands.Dtos;
using TMS.Contract.CQRS.Commands.GenericCommands;
using TMS.Contract.CQRS.Queries.CustomQueries.DepartmentQuries;
using TMS.Contract.CQRS.Queries.GenericQueries;
using Microsoft.Extensions.Logging;

namespace TMS.Server.Controllers;

/// <summary>
/// Controller for managing departments.
/// </summary>
[ApiController]
[Route("[controller]")]
[Authorize]
public class DepartmentsController : ControllerBase
{
    private readonly ISender _sender;
    private readonly ILogger<DepartmentsController> _logger;

    public DepartmentsController(ISender sender, ILogger<DepartmentsController> logger)
    {
        _sender = sender;
        _logger = logger;
    }

    /// <summary>
    /// Creates a new department.
    /// </summary>
    /// <param name="request">Department data.</param>
    /// <param name="token">Cancellation token.</param>
    /// <returns>ApiResponse</returns>
    [HttpPost(DepartmentsEndPoint.Create)]
    [HasPermission(DepartmentManagement.Add)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse>> CreateDepartmentAsync(
        [FromBody] CreateDepartmentDto request,
        CancellationToken token)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Invalid model state for CreateDepartmentAsync");
            return BadRequest(ModelState);
        }
        try
        {
            var result = await _sender.Send(new CreateDepartmentCommand(request), token);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating department");
            return StatusCode(500, "An error occurred while creating the department.");
        }
    }
    
    /// <summary>
    /// Gets all departments.
    /// </summary>
    [HttpGet(DepartmentsEndPoint.GetAll)]
    [HasPermission(DepartmentManagement.Get)]
    [ProducesResponseType(typeof(ApiResponse<List<GetDepartmentResponse>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<List<GetDepartmentResponse>>>> GetAllDepartmentsAsync(CancellationToken token)
    {
        try
        {
            var result = await _sender.Send(new GetAllEntityQuery<Department, GetDepartmentResponse>(
                Include: QueryIncludeHelper.IncludeDepartmentRelations()
            ), token);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all departments");
            return StatusCode(500, "An error occurred while retrieving departments.");
        }
    }

    /// <summary>
    /// Updates the team leader of a department.
    /// </summary>
    [HttpPatch(DepartmentsEndPoint.UpdateDepartmentTeamLeader)]
    [HasPermission(DepartmentManagement.Update)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse>> UpdateDepartmentTeamLeaderAsync(
        [FromQuery] int departmentId,
        [FromQuery] int departmentTeamLeaderId)
    {
        try
        {
            var result = await _sender.Send(new UpdateDepartmentTeamLeaderCommand(departmentId, departmentTeamLeaderId));
            if (result == null)
            {
                _logger.LogWarning("Department not found for UpdateDepartmentTeamLeaderAsync: {DepartmentId}", departmentId);
                return NotFound("Department not found.");
            }
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating department team leader");
            return StatusCode(500, "An error occurred while updating the department team leader.");
        }
    }
    
    /// <summary>
    /// Gets all departments with pagination.
    /// </summary>
    [HttpGet(DepartmentsEndPoint.GetAllPaginated)]
    [HasPermission(DepartmentManagement.Get)]
    [ProducesResponseType(typeof(PaginatedApiResponse<GetDepartmentResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PaginatedApiResponse<GetDepartmentResponse>>> GetAllDepartmentsPaginatedAsync(
        [FromQuery] int pageNumber,
        [FromQuery] int pageSize,
        CancellationToken token)
    {
        if (pageNumber <= 0 || pageSize <= 0)
        {
            _logger.LogWarning("Invalid pagination parameters: pageNumber={PageNumber}, pageSize={PageSize}", pageNumber, pageSize);
            return BadRequest("Invalid pagination parameters.");
        }
        try
        {
            var result = await _sender.Send(new GetAllPaginatedEntityQuery<Department, GetDepartmentResponse>(
                PageSize: pageSize,
                PageNumber: pageNumber,
                Include: QueryIncludeHelper.IncludeDepartmentRelations()
            ), token);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting paginated departments");
            return StatusCode(500, "An error occurred while retrieving paginated departments.");
        }
    }

    /// <summary>
    /// Gets a department by ID.
    /// </summary>
    [HttpGet(DepartmentsEndPoint.Get)]
    [HasPermission(DepartmentManagement.Get)]
    [ProducesResponseType(typeof(ApiResponse<GetDepartmentResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<GetDepartmentResponse>>> GetDepartmentByIdAsync(
        [FromRoute] int departmentId,
        CancellationToken token)
    {
        if (departmentId <= 0)
        {
            _logger.LogWarning("Invalid Department ID: {DepartmentId}", departmentId);
            return BadRequest("Invalid Department ID.");
        }
        try
        {
            var result = await _sender.Send(new GetEntityQuery<Department, GetDepartmentResponse>(
                Filter: x => x.Id == departmentId,
                Include: QueryIncludeHelper.IncludeDepartmentRelations()
            ), token);
            if (result == null)
            {
                _logger.LogWarning("Department not found: {DepartmentId}", departmentId);
                return NotFound("Department not found.");
            }
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting department by ID");
            return StatusCode(500, "An error occurred while retrieving the department.");
        }
    }

    /// <summary>
    /// Updates a department.
    /// </summary>
    [HttpPut(DepartmentsEndPoint.Update)]
    [HasPermission(DepartmentManagement.Update)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse>> UpdateDepartmentAsync(
        [FromRoute] int departmentId,
        [FromBody] UpdateDepartmentDto department,
        CancellationToken token)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Invalid model state for UpdateDepartmentAsync");
            return BadRequest(ModelState);
        }
        try
        {
            var result = await _sender.Send(new UpdateDepartmentCommand(departmentId, department), token);
            if (result == null)
            {
                _logger.LogWarning("Department not found for update: {DepartmentId}", departmentId);
                return NotFound("Department not found.");
            }
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating department");
            return StatusCode(500, "An error occurred while updating the department.");
        }
    }

    /// <summary>
    /// Deletes a department.
    /// </summary>
    [HttpDelete(DepartmentsEndPoint.Delete)]
    [HasPermission(DepartmentManagement.Delete)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse>> DeleteDepartmentAsync(
        [FromRoute] int departmentId,
        CancellationToken token)
    {
        try
        {
            var result = await _sender.Send(new DeleteDepartmentCommand(departmentId), token);
            if (result == null)
            {
                _logger.LogWarning("Department not found for delete: {DepartmentId}", departmentId);
                return NotFound("Department not found.");
            }
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting department");
            return StatusCode(500, "An error occurred while deleting the department.");
        }
    }
}
