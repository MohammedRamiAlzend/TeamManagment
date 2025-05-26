using Microsoft.EntityFrameworkCore;
using TMS.Application.GenericCommands;
using TMS.Application.GenericQueries;

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
        var result = await sender.Send(new GetAllEntityQuery<Employee,EmployeeDto>(Include:x=>x.Include(i=>i.Departments)));
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
    public async Task<IActionResult> UpdateEmployeeAsync(
        [FromRoute] int employeeId,
        [FromBody] UpdateEmployeeDto employee)
    {
        if (employee == null)
        {
            return BadRequest("Employee data is required.");
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await sender.Send(new UpdateEntityCommand<Employee, UpdateEmployeeDto>(employeeId, employee));
        return Ok(result);
    }

    [HttpDelete("{employeeId}")]
    public async Task<IActionResult> DeleteEmployeeAsync([FromRoute] int employeeId)
    {
        var result = await sender.Send(new DeleteEntityCommand<Employee>(x => x.Id == employeeId));
        return Ok(result);
    }
}
