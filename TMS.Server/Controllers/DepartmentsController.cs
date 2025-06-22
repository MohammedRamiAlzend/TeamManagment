using TMS.Contract.CQRS.Commands.CustomCommands.DepartmentCommands;
using TMS.Contract.CQRS.Commands.CustomCommands.DepartmentCommands.Dtos;
using TMS.Contract.CQRS.Commands.GenericCommands;
using TMS.Contract.CQRS.Queries.CustomQueries.DepartmentQuries;
using TMS.Contract.CQRS.Queries.GenericQueries;

namespace TMS.Server.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class DepartmentsController(ISender sender) : ControllerBase
{
    
    [HttpGet(DepartmentsEndPoint.GetAll)]
    [HasPermission(DepartmentManagement.Get)]
    public async Task<ActionResult<ApiResponse<List<GetDepartmentQuery>>>> GetAllDepartmentsAsync(CancellationToken token)
    {
        return await sender.Send(new GetAllEntityQuery<Department, GetDepartmentQuery>(
            Include:QueryIncludeHelper.IncludeDepartmentRelations()
            ), token);
    }

    [HttpPost(DepartmentsEndPoint.UpdateDepartmentTeamLeader)]
    [HasPermission(DepartmentManagement.Update)]
    public async Task<ActionResult<ApiResponse>> UpdateDepartmentTeamLeaderAsync([FromQuery]int departmentId,[FromQuery]int departmentTeamLeaderId)
    {
        return await sender.Send(new UpdateDepartmentTeamLeaderCommand(departmentId,departmentTeamLeaderId));
    }
    
    [HttpGet(DepartmentsEndPoint.GetAllPaginated)]
    [HasPermission(DepartmentManagement.Get)]
    public async Task<ActionResult<PaginatedApiResponse<GetDepartmentQuery>>> GetAllDepartmentsPaginatedAsync(
        [FromQuery] int pageNumber
        , [FromQuery] int pageSize,
        CancellationToken token)
    {
        if (pageNumber <= 0 || pageSize <= 0) return BadRequest("Invalid pagination parameters.");

        return await sender.Send(new GetAllPaginatedEntityQuery<Department, GetDepartmentQuery>(
            PageSize: pageSize,
            PageNumber: pageNumber,
            Include:QueryIncludeHelper.IncludeDepartmentRelations()
            
        ), token);
    }

    [HttpGet(DepartmentsEndPoint.Get)]
    [HasPermission(DepartmentManagement.Get)]
    public async Task<ActionResult<ApiResponse<GetDepartmentQuery>>> GetDepartmentByIdAsync(
        [FromRoute] int departmentId,
        CancellationToken token)
    {
        if (departmentId <= 0) return BadRequest("Invalid Department ID.");
        return await sender.Send(new GetEntityQuery<Department, GetDepartmentQuery>(
            Filter: x => x.Id == departmentId,
            Include:QueryIncludeHelper.IncludeDepartmentRelations()
            ), token);
    }



    [HttpPut(DepartmentsEndPoint.Update)]
    [HasPermission(DepartmentManagement.Update)]
    public async Task<ActionResult<ApiResponse<UpdateDepartmentCommand>>> UpdateDepartmentAsync(
        [FromRoute] int departmentId,
        [FromBody] UpdateDepartmentCommand department,
        CancellationToken token)
    {
        if (department == null) return BadRequest("The Department data must not be null.");
        if (!ModelState.IsValid) return BadRequest(ModelState);
        return await sender.Send(new UpdateEntityCommand<Department, UpdateDepartmentCommand>(departmentId, department,
            Include: QueryIncludeHelper.IncludeDepartmentRelations() ), token);
    }

    [HttpDelete(DepartmentsEndPoint.Delete)]
    [HasPermission(DepartmentManagement.Delete)]
    public async Task<ActionResult<ApiResponse>> DeleteDepartmentAsync(
        [FromRoute] int departmentId,
        CancellationToken token)
    {
        return await sender.Send(new DeleteEntityCommand<Department>(x => x.Id == departmentId), token);
    }
}
