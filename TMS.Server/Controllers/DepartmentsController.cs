using System.Linq.Expressions;
using Microsoft.AspNetCore.WebUtilities;
using TMS.Infrastructure.Helpers;

namespace TMS.Server.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class DepartmentsController(ISender sender) : ControllerBase
{
    
    [HttpGet(DepartmentsEndPoint.GetAll)]
    [HasPermission(DepartmentManagement.Get)]
    public async Task<ActionResult<ApiResponse<List<GetDepartmentResponse>>>> GetAllDepartmentsAsync(CancellationToken token)
    {
        return await sender.Send(new GetAllEntityQuery<Department, GetDepartmentResponse>(
            Include:QueryIncludeHelper.IncludeDepartmentRelations()
            ), token);
    }

    [HttpGet(DepartmentsEndPoint.GetAllPaginated)]
    [HasPermission(DepartmentManagement.Get)]
    public async Task<ActionResult<PaginatedApiResponse<GetDepartmentResponse>>> GetAllDepartmentsPaginatedAsync(
        [FromQuery] int pageNumber
        , [FromQuery] int pageSize,
        CancellationToken token)
    {
        if (pageNumber <= 0 || pageSize <= 0) return BadRequest("Invalid pagination parameters.");

        return await sender.Send(new GetAllPaginatedEntityQuery<Department, GetDepartmentResponse>(
            PageSize: pageSize,
            PageNumber: pageNumber,
            Include:QueryIncludeHelper.IncludeDepartmentRelations()
            
        ), token);
    }

    [HttpGet(DepartmentsEndPoint.Get)]
    [HasPermission(DepartmentManagement.Get)]
    public async Task<ActionResult<ApiResponse<GetDepartmentResponse>>> GetDepartmentByIdAsync(
        [FromRoute] int departmentId,
        CancellationToken token)
    {
        if (departmentId <= 0) return BadRequest("Invalid Department ID.");
        return await sender.Send(new GetEntityQuery<Department, GetDepartmentResponse>(
            Filter: x => x.Id == departmentId,
            Include:QueryIncludeHelper.IncludeDepartmentRelations()
            ), token);
    }



    [HttpPut(DepartmentsEndPoint.Update)]
    [HasPermission(DepartmentManagement.Update)]
    public async Task<ActionResult<ApiResponse<UpdateDepartmentDto>>> UpdateDepartmentAsync(
        [FromRoute] int departmentId,
        [FromBody] UpdateDepartmentDto department,
        CancellationToken token)
    {
        if (department == null) return BadRequest("The Department data must not be null.");
        if (!ModelState.IsValid) return BadRequest(ModelState);
        return await sender.Send(new UpdateEntityCommand<Department, UpdateDepartmentDto>(departmentId, department,
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
