using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using TMS.Application.GenericCommands;
using TMS.Application.GenericQueries;
using TMS.Core;
using TMS.Core.AutoMapperClasses.DTOs.RequestDTOs.RequestEmployeeDTOS;
using TMS.Core.AutoMapperClasses.DTOs.ResponseDTOs.ResponseEmployeeDTOS;

namespace TMS.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class EmployeesController(ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<EmployeeDto>>>> GetAllEmployeesAsync()
    {
        return await sender.Send(new GetAllEntityQuery<Employee,EmployeeDto>(Include:x=>x.Include(i=>i.Departments)));
    }
    [HttpGet("paginated")]
    public async Task<ActionResult<PaginatedApiResponse<EmployeeDto>>> GetAllEmployeesPaginatedAsync([FromQuery]int pageNumber,[FromQuery]int pageSize)
    {
        if (pageNumber <= 0 || pageSize <= 0) return BadRequest("Invalid pagination parameters.");

      return await sender.Send(new GetAllPaginatedEntityQuery<Employee, EmployeeDto>(
                                                                                PageSize: pageSize));
    }
    [HttpGet("{employeeId:int}")]
    public async Task<ActionResult<ApiResponse<EmployeeDto>>> GetEmployeeByIdAsync([FromRoute] int employeeId)
    {
        if (employeeId <= 0) return BadRequest("Invalid employee ID.");

        return await sender.Send(new GetEntityQuery<Employee, EmployeeDto>(
                Filter: x => x.Id == employeeId));
    }
    [HttpPut("{employeeId:int}")]
    public async Task<ActionResult<ApiResponse<UpdateEmployeeDto>>> UpdateEmployeeAsync(
        [FromRoute] int employeeId,
        [FromBody] UpdateEmployeeDto employee)
    {
        if (employee == null) return BadRequest("The employee data must not be null.");
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        return await sender.Send(new UpdateEntityCommand<Employee, UpdateEmployeeDto>(employeeId, employee));
    }

    [HttpDelete("{employeeId:int}")]
    public async Task<ActionResult<ApiResponse>> DeleteEmployeeAsync([FromRoute] int employeeId)
    {
        return await sender.Send(new DeleteEntityCommand<Employee>(x => x.Id == employeeId));
    }
}
