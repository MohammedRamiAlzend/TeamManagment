using TMS.Contract.CQRS.Queries.CustomQueries.RoleQuries;
using TMS.Contract.CQRS.Queries.GenericQueries;

namespace TMS.Server.Controllers;

[ApiController]
[Route($"{ApiBase}/[controller]")]
public class RolesController(ISender sender)
{
    [HttpGet(RolesEndPoint.GetAll)]
    public async Task<ActionResult<ApiResponse<List<GetRoleResponse>>>> GetAllRoleAsync(CancellationToken token)
    {
        return await sender.Send(new GetAllEntityQuery<Role, GetRoleResponse>(Include:x=>x.Include(i=>i.Permissions)),token);
    }
    [HttpGet(RolesEndPoint.Get)]
    public async Task<ActionResult<ApiResponse<GetRoleResponse>>> GetRoleAsync(
        [FromRoute] int roleId,
        CancellationToken token)
    {
        return await sender.Send(new GetEntityQuery<Role, GetRoleResponse>(
            Filter: x=>x.Id == roleId,
            Include:x=>x.Include(i=>i.Permissions)), token);
    }
    [HttpGet(RolesEndPoint.GetAllPaginated)]
    public async Task<ActionResult<PaginatedApiResponse<GetRoleResponse>>> GetRoleAsync(
        [FromQuery] int pageSize,
        [FromQuery] int pageNumber,
        CancellationToken token
    )
    {
        return await sender.Send(new GetAllPaginatedEntityQuery<Role, GetRoleResponse>(
                PageNumber:pageNumber,
                PageSize: pageSize,
                Include:x=>x.Include(i=>i.Permissions)),
            token);
    }
}