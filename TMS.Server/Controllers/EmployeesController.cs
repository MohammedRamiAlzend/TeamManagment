using TMS.Contract.CQRS.Commands.CustomCommands.EmployeeCommands.Dtos;
using TMS.Contract.CQRS.Commands.GenericCommands;
using TMS.Contract.CQRS.Queries.CustomQueries.EmployeeQuries;
using TMS.Contract.CQRS.Queries.GenericQueries;
using Microsoft.Extensions.Logging;
using TMS.Application.Handlers.CustomHandlers.EmployeeHandlers;

namespace TMS.Server.Controllers;

/// <summary>
/// Controller for managing employees.
/// </summary>
[ApiController]
[Route($"{ApiBase}/[controller]")]
[Authorize]
public class EmployeesController : ControllerBase
{
    private readonly ISender _sender;
    private readonly ILogger<EmployeesController> _logger;

    public EmployeesController(ISender sender, ILogger<EmployeesController> logger)
    {
        _sender = sender;
        _logger = logger;
    }

    /// <summary>
    /// Gets all employees.
    /// </summary>
    [HttpGet(EmployeesEndPoint.GetAll)]
    [HasPermission(EmployeeManagement.Get)]
    [ProducesResponseType(typeof(ApiResponse<List<GetEmployeeResponse>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<List<GetEmployeeResponse>>>> GetAllEmployeesAsync(CancellationToken token)
    {
        try
        {
            var result = await _sender.Send(new GetAllEntityQuery<Employee, GetEmployeeResponse>(
                Include: QueryIncludeHelper.IncludeEmployeeRelations()
            ), token);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all employees");
            return StatusCode(500, "An error occurred while retrieving employees.");
        }
    }

    /// <summary>
    /// Gets all employees with pagination.
    /// </summary>
    [HttpGet(EmployeesEndPoint.GetAllPaginated)]
    [HasPermission(EmployeeManagement.Get)]
    [ProducesResponseType(typeof(PaginatedApiResponse<GetEmployeeResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PaginatedApiResponse<GetEmployeeResponse>>> GetAllEmployeesPaginatedAsync(
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
            var result = await _sender.Send(
                new GetAllPaginatedEntityQuery<Employee, GetEmployeeResponse>(
                    PageNumber: pageNumber, PageSize: pageSize,
                    Include: QueryIncludeHelper.IncludeEmployeeRelations()
                ), token);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting paginated employees");
            return StatusCode(500, "An error occurred while retrieving paginated employees.");
        }
    }

    /// <summary>
    /// Gets an employee by ID.
    /// </summary>
    [HttpGet(EmployeesEndPoint.Get)]
    [HasPermission(EmployeeManagement.Get)]
    [ProducesResponseType(typeof(ApiResponse<GetEmployeeResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<GetEmployeeResponse>>> GetEmployeeByIdAsync(
        [FromRoute] int employeeId,
        CancellationToken token)
    {
        if (employeeId <= 0)
        {
            _logger.LogWarning("Invalid employee ID: {EmployeeId}", employeeId);
            return BadRequest("Invalid employee ID.");
        }
        try
        {
            var result = await _sender.Send(new GetEntityQuery<Employee, GetEmployeeResponse>(
                x => x.Id == employeeId,
                Include: QueryIncludeHelper.IncludeEmployeeRelations()
            ), token);
            if (result == null)
            {
                _logger.LogWarning("Employee not found: {EmployeeId}", employeeId);
                return NotFound("Employee not found.");
            }
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting employee by ID");
            return StatusCode(500, "An error occurred while retrieving the employee.");
        }
    }

    /// <summary>
    /// Updates an employee.
    /// </summary>
    [HttpPut(EmployeesEndPoint.Update)]
    [HasPermission(EmployeeManagement.Update)]
    [ProducesResponseType(typeof(ApiResponse<UpdateEmployeeDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<UpdateEmployeeDto>>> UpdateEmployeeAsync(
        [FromRoute] int employeeId,
        [FromBody] UpdateEmployeeDto employee,
        CancellationToken token)
    {
        if (employee == null)
        {
            _logger.LogWarning("The employee data must not be null.");
            return BadRequest("The employee data must not be null.");
        }
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Invalid model state for UpdateEmployeeAsync");
            return BadRequest(ModelState);
        }
        try
        {
            var result = await _sender.Send(new UpdateEntityCommand<Employee, UpdateEmployeeDto>(x => x.Id == employeeId, employee), token);
            if (result == null)
            {
                _logger.LogWarning("Employee not found for update: {EmployeeId}", employeeId);
                return NotFound("Employee not found.");
            }
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating employee");
            return StatusCode(500, "An error occurred while updating the employee.");
        }
    }

    /// <summary>
    /// Deletes an employee.
    /// </summary>
    [HttpDelete(EmployeesEndPoint.Delete)]
    [HasPermission(EmployeeManagement.Delete)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse>> DeleteEmployeeAsync(
        [FromRoute] int employeeId,
        CancellationToken token)
    {
        try
        {
            var result = await _sender.Send(new DeleteEntityCommand<Employee>(x => x.Id == employeeId), token);
            if (result == null)
            {
                _logger.LogWarning("Employee not found for delete: {EmployeeId}", employeeId);
                return NotFound("Employee not found.");
            }
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting employee");
            return StatusCode(500, "An error occurred while deleting the employee.");
        }
    }

    /// <summary>
    /// Deletes an employee and sets related tasks' CreatedByEmployeeId to null.
    /// </summary>
    [HttpDelete("custom-delete/{employeeId:int}")]
    [HasPermission(EmployeeManagement.Delete)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse>> DeleteEmployeeCustomAsync(
        [FromRoute] int employeeId,
        CancellationToken token)
    {
        try
        {
            var result = await _sender.Send(new DeleteEmployeeCommand(employeeId), token);
            if (!result)
            {
                _logger.LogWarning("Employee not found for custom delete: {EmployeeId}", employeeId);
                return NotFound(ApiResponse.Failure(System.Net.HttpStatusCode.NotFound, "Employee not found."));
            }
            return Ok(ApiResponse.Success(System.Net.HttpStatusCode.OK, "Employee deleted and related tasks updated."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting employee (custom handler)");
            return StatusCode(500, ApiResponse.Failure(System.Net.HttpStatusCode.InternalServerError, "An error occurred while deleting the employee."));
        }
    }
}