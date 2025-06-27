using TMS.Contract.CQRS.Queries.CustomQueries.PermissionQuries;
using TMS.Contract.CQRS.Queries.GenericQueries;
using Microsoft.Extensions.Logging;

namespace TMS.Server.Controllers;

/// <summary>
/// Controller for managing permissions.
/// </summary>
[ApiController]
[Route($"{ApiBase}/[controller]")]
public class PermissionsController : ControllerBase
{
    private readonly ISender _sender;
    private readonly ILogger<PermissionsController> _logger;

    public PermissionsController(ISender sender, ILogger<PermissionsController> logger)
    {
        _sender = sender;
        _logger = logger;
    }

    /// <summary>
    /// Gets all permissions.
    /// </summary>
    [HttpGet(PermissionsEndPoint.GetAll)]
    [ProducesResponseType(typeof(ApiResponse<List<GetPermissionResponse>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<List<GetPermissionResponse>>>> GetAllPermissionsAsync(CancellationToken token)
    {
        try
        {
            var result = await _sender.Send(new GetAllEntityQuery<Permission, GetPermissionResponse>(Include: x => x.Include(i => i.Roles)), token);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all permissions");
            return StatusCode(500, "An error occurred while retrieving permissions.");
        }
    }

    /// <summary>
    /// Gets a permission by ID.
    /// </summary>
    [HttpGet(PermissionsEndPoint.Get)]
    [ProducesResponseType(typeof(ApiResponse<GetPermissionResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<GetPermissionResponse>>> GetPermissionAsync(
        [FromRoute] int permissionId,
        CancellationToken token)
    {
        try
        {
            var result = await _sender.Send(new GetEntityQuery<Permission, GetPermissionResponse>(
                Filter: x => x.Id == permissionId,
                Include: x => x.Include(i => i.Roles)), token);
            if (result == null)
            {
                _logger.LogWarning("Permission not found: {PermissionId}", permissionId);
                return NotFound("Permission not found.");
            }
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting permission by ID");
            return StatusCode(500, "An error occurred while retrieving the permission.");
        }
    }

    /// <summary>
    /// Gets all permissions with pagination.
    /// </summary>
    [HttpGet(PermissionsEndPoint.GetAllPaginated)]
    [ProducesResponseType(typeof(PaginatedApiResponse<GetPermissionResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PaginatedApiResponse<GetPermissionResponse>>> GetPermissionAsync(
            [FromQuery] int pageSize,
            [FromQuery] int pageNumber,
            CancellationToken token
        )
    {
        try
        {
            var result = await _sender.Send(new GetAllPaginatedEntityQuery<Permission, GetPermissionResponse>(
                    PageNumber: pageNumber,
                    PageSize: pageSize,
                    Include: x => x.Include(i => i.Roles)),
                token);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting paginated permissions");
            return StatusCode(500, "An error occurred while retrieving paginated permissions.");
        }
    }
}