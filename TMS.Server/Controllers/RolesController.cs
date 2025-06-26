using TMS.Contract.CQRS.Queries.CustomQueries.RoleQuries;
using TMS.Contract.CQRS.Queries.GenericQueries;
using Microsoft.Extensions.Logging;

namespace TMS.Server.Controllers;

/// <summary>
/// Controller for managing roles.
/// </summary>
[ApiController]
[Route($"{ApiBase}/[controller]")]
public class RolesController : ControllerBase
{
    private readonly ISender _sender;
    private readonly ILogger<RolesController> _logger;

    public RolesController(ISender sender, ILogger<RolesController> logger)
    {
        _sender = sender;
        _logger = logger;
    }

    /// <summary>
    /// Gets all roles.
    /// </summary>
    [HttpGet(RolesEndPoint.GetAll)]
    [ProducesResponseType(typeof(ApiResponse<List<GetRoleResponse>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<List<GetRoleResponse>>>> GetAllRoleAsync(CancellationToken token)
    {
        try
        {
            var result = await _sender.Send(new GetAllEntityQuery<Role, GetRoleResponse>(Include: x => x.Include(i => i.Permissions)), token);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all roles");
            return StatusCode(500, "An error occurred while retrieving roles.");
        }
    }

    /// <summary>
    /// Gets a role by ID.
    /// </summary>
    [HttpGet(RolesEndPoint.Get)]
    [ProducesResponseType(typeof(ApiResponse<GetRoleResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<GetRoleResponse>>> GetRoleAsync(
        [FromRoute] int roleId,
        CancellationToken token)
    {
        try
        {
            var result = await _sender.Send(new GetEntityQuery<Role, GetRoleResponse>(
                Filter: x => x.Id == roleId,
                Include: x => x.Include(i => i.Permissions)), token);
            if (result == null)
            {
                _logger.LogWarning("Role not found: {RoleId}", roleId);
                return NotFound("Role not found.");
            }
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting role by ID");
            return StatusCode(500, "An error occurred while retrieving the role.");
        }
    }

    /// <summary>
    /// Gets all roles with pagination.
    /// </summary>
    [HttpGet(RolesEndPoint.GetAllPaginated)]
    [ProducesResponseType(typeof(PaginatedApiResponse<GetRoleResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PaginatedApiResponse<GetRoleResponse>>> GetRoleAsync(
        [FromQuery] int pageSize,
        [FromQuery] int pageNumber,
        CancellationToken token
    )
    {
        try
        {
            var result = await _sender.Send(new GetAllPaginatedEntityQuery<Role, GetRoleResponse>(
                    PageNumber: pageNumber,
                    PageSize: pageSize,
                    Include: x => x.Include(i => i.Permissions)),
                token);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting paginated roles");
            return StatusCode(500, "An error occurred while retrieving paginated roles.");
        }
    }
}