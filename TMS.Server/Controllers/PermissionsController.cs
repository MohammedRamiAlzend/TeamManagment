using TMS.Contract.CQRS.Queries.CustomQueries.PermissionQuries;
using TMS.Contract.CQRS.Queries.GenericQueries;

namespace TMS.Server.Controllers;

[ApiController]
[Route($"{ApiBase}/[controller]")]
public class PermissionsController(ISender sender)
{
    [HttpGet(PermissionsEndPoint.GetAll)]
    public async Task<ActionResult<ApiResponse<List<GetPermissionResponse>>>> GetAllPermissionsAsync(CancellationToken token)
    {
        return await sender.Send(new GetAllEntityQuery<Permission, GetPermissionResponse>(Include:x=>x.Include(i=>i.Roles)),token);
    }
    [HttpGet(PermissionsEndPoint.Get)]
    public async Task<ActionResult<ApiResponse<GetPermissionResponse>>> GetPermissionAsync(
        [FromRoute] int permissionId,
        CancellationToken token)
    {
        return await sender.Send(new GetEntityQuery<Permission, GetPermissionResponse>(
            Filter: x=>x.Id == permissionId,
            Include:x=>x.Include(i=>i.Roles)), token);
    }
    [HttpGet(PermissionsEndPoint.GetAllPaginated)]
    public async Task<ActionResult<PaginatedApiResponse<GetPermissionResponse>>> GetPermissionAsync(
            [FromQuery] int pageSize,
            [FromQuery] int pageNumber,
            CancellationToken token
        )
    {
        return await sender.Send(new GetAllPaginatedEntityQuery<Permission, GetPermissionResponse>(
                PageNumber:pageNumber,
                PageSize: pageSize,
                Include:x=>x.Include(i=>i.Roles)),
            token);
    }
}