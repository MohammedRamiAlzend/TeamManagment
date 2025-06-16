using System.Linq.Expressions;
using TMS.Server.Controllers.ControllersHelper;
using static TMS.Server.Controllers.ControllersHelper.IncludeHelper;

namespace TMS.Server.Controllers;

[ApiController]
[Route($"{ApiBase}/[controller]")]
[Authorize]
public class EmployeesController(ISender sender) : ControllerBase
{

    [HttpGet(EmployeesEndPoint.GetAll)]
    [HasPermission(EmployeeManagement.Get)]
    public async Task<ActionResult<ApiResponse<List<GetEmployeeResponse>>>> GetAllEmployeesAsync([FromQuery]string[]? includes, CancellationToken token)
    {
        return await sender.Send(new GetAllEntityQuery<Employee, GetEmployeeResponse>(), token);
        // return await sender.Send(new GetAllEntityQuery<Employee, GetEmployeeResponse>(
        //     Include: GetIncludes(includes,IncludeExpressions)), token);
    }

    [HttpGet(EmployeesEndPoint.GetAllPaginated)]
    [HasPermission(EmployeeManagement.Get)]
    
    public async Task<ActionResult<PaginatedApiResponse<GetEmployeeResponse>>> GetAllEmployeesPaginatedAsync(
        [FromQuery] int pageNumber,
        [FromQuery] int pageSize,
        CancellationToken token)
    {
        if (pageNumber <= 0 || pageSize <= 0) return BadRequest("Invalid pagination parameters.");

        return await sender.Send(
            new GetAllPaginatedEntityQuery<Employee, GetEmployeeResponse>(
                PageNumber: pageNumber, PageSize: pageSize
                , Include: x => x.Include(x => x.Departments)!
                    .ThenInclude(s => s.SubDepartments)), token);
    }

    [HttpGet(EmployeesEndPoint.Get)]
    [HasPermission(EmployeeManagement.Get)]
    public async Task<ActionResult<ApiResponse<GetEmployeeResponse>>> GetEmployeeByIdAsync(
        [FromRoute] int employeeId,
        [FromQuery] string[]? includes,
        CancellationToken token)
    {
        if (employeeId <= 0) return BadRequest("Invalid employee ID.");

        return await sender.Send(new GetEntityQuery<Employee, GetEmployeeResponse>(
            x => x.Id == employeeId,
            Include: GetIncludes(includes,IncludeExpressions)), token);
    }

    [HttpPut(EmployeesEndPoint.Update)]
    [HasPermission(EmployeeManagement.Update)]
    public async Task<ActionResult<ApiResponse<UpdateEmployeeDto>>> UpdateEmployeeAsync(
        [FromRoute] int employeeId,
        [FromBody] UpdateEmployeeDto employee,
        CancellationToken token)
    {
        if (employee == null) return BadRequest("The employee data must not be null.");
        if (!ModelState.IsValid) return BadRequest(ModelState);
        return await sender.Send(new UpdateEntityCommand<Employee, UpdateEmployeeDto>(employeeId, employee), token);
    }

    [HttpDelete(EmployeesEndPoint.Delete)]
    [HasPermission(EmployeeManagement.Delete)]
    public async Task<ActionResult<ApiResponse>> DeleteEmployeeAsync(
        [FromRoute] int employeeId,
        CancellationToken token)
    {
        return await sender.Send(new DeleteEntityCommand<Employee>(x => x.Id == employeeId), token);
    }
    

    [HttpGet(EmployeesEndPoint.Includes)]
    [AllowAnonymous]
    public ActionResult<IEnumerable<string>> IncludesResult()
    {
        return Ok(IncludeExpressions.Keys);
    }

    private static readonly Dictionary<string, Expression<Func<Employee, object>>> IncludeExpressions = new()
    {
        [nameof(Employee.Departments)] = e => e.Departments,
        [nameof(Employee.User)] = e => e.User,
        [nameof(Employee.CreatedTasks)] = e => e.CreatedTasks,
        [nameof(Employee.AssignedTasks)] = e => e.AssignedTasks,
    };
  

}