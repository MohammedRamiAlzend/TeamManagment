using System.Linq.Expressions;

using static TMS.Server.Controllers.ControllersHelper.IncludeHelper;

namespace TMS.Server.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class DepartmentsController(ISender sender) : ControllerBase
{
    
    [HttpGet(DepartmentsEndPoint.GetAll)]
    [HasPermission(DepartmentManagement.Get)]
    public async Task<ActionResult<ApiResponse<List<DepartmentDto>>>> GetAllDepartmentsAsync(CancellationToken token)
    {
        return await sender.Send(new GetAllEntityQuery<Department, DepartmentDto>(
            Include: x => x.Include(i => i.Employees)
                .Include(i2 => i2.SubDepartments)
                .Include(i3 => i3.TeamLeader)), token);
    }

    [HttpGet(DepartmentsEndPoint.GetAllPaginated)]
    [HasPermission(DepartmentManagement.Get)]
    public async Task<ActionResult<PaginatedApiResponse<DepartmentDto>>> GetAllDepartmentsPaginatedAsync(
        [FromQuery] int pageNumber
        , [FromQuery] int pageSize,
        CancellationToken token)
    {
        if (pageNumber <= 0 || pageSize <= 0) return BadRequest("Invalid pagination parameters.");

        return await sender.Send(new GetAllPaginatedEntityQuery<Department, DepartmentDto>(
            PageSize: pageSize,
            PageNumber: pageNumber,
            Include: x => x.Include(i => i.Employees)
                .Include(d2 => d2.SubDepartments)
                .Include(d3 => d3.TeamLeader)
        ), token);
    }

    [HttpGet(DepartmentsEndPoint.Get)]
    [HasPermission(DepartmentManagement.Get)]
    public async Task<ActionResult<ApiResponse<DepartmentDto>>> GetDepartmentByIdAsync(
        [FromRoute] int departmentId,
        [FromQuery] string[]? includes,
        CancellationToken token)
    {
        if (departmentId <= 0) return BadRequest("Invalid Department ID.");
        return await sender.Send(new GetEntityQuery<Department, DepartmentDto>(
            Include: GetIncludes(includes,IncludeExpressions),
            Filter: x => x.Id == departmentId), token);
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
        return await sender.Send(new UpdateEntityCommand<Department, UpdateDepartmentDto>(departmentId, department),
            token);
    }

    [HttpDelete(DepartmentsEndPoint.Delete)]
    [HasPermission(DepartmentManagement.Delete)]
    public async Task<ActionResult<ApiResponse>> DeleteDepartmentAsync(
        [FromRoute] int departmentId,
        CancellationToken token)
    {
        return await sender.Send(new DeleteEntityCommand<Department>(x => x.Id == departmentId), token);
    }

    [HttpGet(DepartmentsEndPoint.Includes)]
    [AllowAnonymous]
    public ActionResult<IEnumerable<string>> GetDepartmentIncludes()
    {
        return Ok(IncludeExpressions.Keys);
    }
    
    
    private static readonly Dictionary<string, Expression<Func<Department, object>>> IncludeExpressions = new()
    {
        { nameof(Department.Employees), d => d.Employees },
        { nameof(Department.SubDepartments), d => d.SubDepartments! },
        { nameof(Department.TeamLeader), d => d.TeamLeader }
    };
}
