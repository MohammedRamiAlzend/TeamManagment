using TMS.Core.CommunicationModels;

namespace TMS.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class DepartmentsController(ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<DepartmentDto>>>> GetAllDepartmentsAsync(CancellationToken token)
    {
        return await sender.Send(new GetAllEntityQuery<Department, DepartmentDto>(
            Include: x => x.Include(i => i.Employees)
                .Include(i2 => i2.SubDepartments)
                .Include(i3 => i3.TeamLeader)), token);
    }

    [HttpGet("paginated")]
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
                .Include(i2 => i2.SubDepartments)
                .Include(x => x.TeamLeader)
        ), token);
    }

    [HttpGet("{departmentId:int}")]
    public async Task<ActionResult<ApiResponse<DepartmentDto>>> GetDepartmentByIdAsync(
        [FromRoute] int departmentId,
        [FromQuery] string[]? includes,
        CancellationToken token)
    {
        if (departmentId <= 0) return BadRequest("Invalid Department ID.");
        return await sender.Send(new GetEntityQuery<Department, DepartmentDto>(
            Include: GetIncludes(includes),
            Filter: x => x.Id == departmentId), token);
    }


    [HttpGet("departments/includes")]
    public ActionResult<IEnumerable<string>> GetDepartmentIncludes()
    {
        var includes = new[]
        {
            nameof(Department.Employees),
            nameof(Department.SubDepartments),
            nameof(Department.TeamLeader)
        };
        return Ok(includes);
    }

    [HttpPut("{departmentId:int}")]
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

    [HttpDelete("{departmentId:int}")]
    public async Task<ActionResult<ApiResponse>> DeleteDepartmentAsync(
        [FromRoute] int departmentId,
        CancellationToken token)
    {
        return await sender.Send(new DeleteEntityCommand<Department>(x => x.Id == departmentId), token);
    }


    private static Func<IQueryable<Department>, IIncludableQueryable<Department, object>>? GetIncludes(
        string[]? includeProperties)
    {
        if (includeProperties is null || includeProperties.Length == 0) return null;

        return query =>
        {
            if (includeProperties.Contains(nameof(Department.Employees)))
                query = query.Include(d => d.Employees);
            if (includeProperties.Contains(nameof(Department.SubDepartments)))
                query = query.Include(d => d.SubDepartments);
            if (includeProperties.Contains(nameof(Department.TeamLeader)))
                query = query.Include(d => d.TeamLeader);
            return (IIncludableQueryable<Department, object>)query;
        };
    }
}