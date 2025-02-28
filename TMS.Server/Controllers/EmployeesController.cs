using TMS.Application.Commands;
using TMS.Application.Queries;

namespace TMS.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class EmployeesController(ISender sender) : ControllerBase
{
    [HttpPost("AddEmployee")]
    public async Task<IActionResult> AddEmployeeAsync([FromBody] CreateEmployeeDto employee)
    {
        var result = await sender.Send(new AddEntityCommand<CreateEmployeeDto>(employee));
        return Ok(result);
    }

    [HttpGet("GetAllEmployees")]
    public async Task<IActionResult> GetAllEmployeesAsync()
    {
        var result = await sender.Send(new GetAllEntityQuery<Employee,EmployeeDto>());
        return Ok(result);
    }
    [HttpGet("GetAllEmployeesPaginated/{pageNumber}/{pageSize}")]
    public async Task<IActionResult> GetAllEmployeesPaginatedAsync([FromRoute]int pageNumber,[FromRoute] int pageSize)
    {
        var result = await sender.Send(new GetAllPaginatedEntityQuery<Employee, EmployeeDto>(
                                                                                PageSize: pageSize));
        return Ok(result);
    }
    [HttpGet("{employeeId}")]
    public async Task<IActionResult> GetEmployeeByIdAsync([FromRoute] int employeeId)
    {
        var result = await sender.Send(new GetEntityQuery<Employee, EmployeeDto>(
                Filter: x => x.Id == employeeId));
        return Ok(result);
    }
    [HttpPut("{employeeId}")]
    public async Task<IActionResult> UpdateEmployeeAsync([FromRoute] int employeeId, [FromBody] CreateEmployeeDto employee)
    {
        var result = await sender.Send(new UpdateEntityCommand<Employee,CreateEmployeeDto>(employeeId,employee));
        return Ok(result);
    }

    [HttpDelete("{employeeId}")]
    public async Task<IActionResult> DeleteEmployeeAsync([FromRoute] int employeeId)
    {
        var result = await sender.Send(new DeleteEntityCommand<Employee>(x => x.Id == employeeId));
        return Ok(result);
    }
}
