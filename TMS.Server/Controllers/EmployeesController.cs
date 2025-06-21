using System.Linq.Expressions;
using TMS.Infrastructure.Helpers;
using TMS.Server.Controllers.ControllersHelper;

namespace TMS.Server.Controllers;

[ApiController]
[Route($"{ApiBase}/[controller]")]
[Authorize]
public class EmployeesController(ISender sender) : ControllerBase
{

    [HttpGet(EmployeesEndPoint.GetAll)]
    [HasPermission(EmployeeManagement.Get)]
    public async Task<ActionResult<ApiResponse<List<GetEmployeeResponse>>>> GetAllEmployeesAsync(CancellationToken token)
    {
        return await sender.Send(new GetAllEntityQuery<Employee, GetEmployeeResponse>(
                Include:QueryIncludeHelper.IncludeEmployeeRelations()
            ), token);
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
                PageNumber: pageNumber, PageSize: pageSize,
                Include:QueryIncludeHelper.IncludeEmployeeRelations()
                ), token);
    }

    [HttpGet(EmployeesEndPoint.Get)]
    [HasPermission(EmployeeManagement.Get)]
    public async Task<ActionResult<ApiResponse<GetEmployeeResponse>>> GetEmployeeByIdAsync(
        [FromRoute] int employeeId,
        CancellationToken token)
    {
        if (employeeId <= 0) return BadRequest("Invalid employee ID.");

        return await sender.Send(new GetEntityQuery<Employee, GetEmployeeResponse>(
            x => x.Id == employeeId,
            Include:QueryIncludeHelper.IncludeEmployeeRelations()
            ), token);
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
}