using Microsoft.AspNetCore.Mvc;
using TMS.Application.Commands;
using TMS.Application.Queries;
using TMS.Core.AutoMapperClasses.DTOs;
using TMS.Core.Entities;
using TMS.Core.MediatR.Interfaces;

namespace TMS.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EmployeesController(ISender sender, ILogger<EmployeesController> logger) : ControllerBase
    {
        [HttpPost("")]
        public async Task<IActionResult> AddEmployeeAsync([FromBody] EmployeeDTO employee)
        {
            var result = await sender.Send(new AddEntityCommand<EmployeeDTO>(employee));
            return Ok(result);
        }

        [HttpGet("")]
        public async Task<IActionResult> GetAllEmployeesAsync()
        {
            var result = await sender.Send(new GetAllEntityQuery<Employee,EmployeeDTO>());
            return Ok(result);
        }
        [HttpGet("{employeeId}")]
        public async Task<IActionResult> GetEmployeeByIdAsync([FromRoute] int employeeId)
        {
            var result = await sender.Send(new GetEntityQuery<Employee,EmployeeDTO>(
                    Filter: x => x.Id == employeeId
                ));
            return Ok(result);
        }
        [HttpPut("{employeeId}")]
        public async Task<IActionResult> UpdateEmployeeAsync([FromRoute] int employeeId, [FromBody] EmployeeDTO employee)
        {
            var result = await sender.Send(new UpdateEntityCommand<Employee,EmployeeDTO>(employeeId,employee));
            return Ok(result);
        }

        [HttpDelete("{employeeId}")]
        public async Task<IActionResult> DeleteEmployeeAsync([FromRoute] int employeeId)
        {
            var result = await sender.Send(new DeleteEntityCommand<Employee>(x => x.Id == employeeId));
            return Ok(result);
        }
    }
}
