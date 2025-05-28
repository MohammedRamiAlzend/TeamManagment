using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using TMS.Application.GenericCommands;
using TMS.Application.GenericQueries;
using TMS.Core;
using TMS.Core.AutoMapperClasses.DTOs.RequestDTOs.RequestDepartmentsDTOS;
using TMS.Core.AutoMapperClasses.DTOs.RequestDTOs.RequestEmployeeDTOS;
using TMS.Core.AutoMapperClasses.DTOs.ResponseDTOs.ResponseDepartmentDTOS;
using TMS.Core.AutoMapperClasses.DTOs.ResponseDTOs.ResponseEmployeeDTOS;

namespace TMS.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class DepartmentsController(ISender sender) : ControllerBase
{
    
    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<DepartmentDto>>>> GetAllDepartmentsAsync()
    {
        return await sender.Send(new GetAllEntityQuery<Department,DepartmentDto>(
            Include:x=>x.Include(i=>i.Employees)
                                              .Include(i2=>i2.SubDepartments)
                                              .Include(x=>x.TeamLeader))
        );
    }
    [HttpGet("paginated")]
    public async Task<ActionResult<PaginatedApiResponse<DepartmentDto>>> GetAllDepartmentsPaginatedAsync([FromQuery]int pageNumber,[FromQuery]int pageSize)
    {
        if (pageNumber <= 0 || pageSize <= 0) return BadRequest("Invalid pagination parameters.");

      return await sender.Send(new GetAllPaginatedEntityQuery<Department, DepartmentDto>(
                                                                                PageSize: pageSize));
    }
    [HttpGet("{departmentId:int}")]
    public async Task<ActionResult<ApiResponse<DepartmentDto>>> GetDepartmentByIdAsync([FromRoute] int departmentId)
    {
        if (departmentId <= 0) return BadRequest("Invalid Department ID.");

        return await sender.Send(new GetEntityQuery<Department, DepartmentDto>(
                Filter: x => x.Id == departmentId));
    }
    [HttpPut("{departmentId:int}")]
    public async Task<ActionResult<ApiResponse<UpdateDepartmentDto>>> UpdateDepartmentAsync(
        [FromRoute] int departmentId,
        [FromBody] UpdateDepartmentDto department)
    {
        if (department == null) return BadRequest("The Department data must not be null.");
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        return await sender.Send(new UpdateEntityCommand<Department, UpdateDepartmentDto>(departmentId, department));
    }

    [HttpDelete("{departmentId:int}")]
    public async Task<ActionResult<ApiResponse>> DeleteDepartmentAsync([FromRoute] int departmentId)
    {
        return await sender.Send(new DeleteEntityCommand<Department>(x => x.Id == departmentId));
    }
}
