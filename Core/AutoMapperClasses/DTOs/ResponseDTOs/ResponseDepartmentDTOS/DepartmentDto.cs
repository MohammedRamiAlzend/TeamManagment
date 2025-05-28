using TMS.Core.AutoMapperClasses.DTOs.ResponseDTOs.ResponseEmployeeDTOS;

namespace TMS.Core.AutoMapperClasses.DTOs.ResponseDTOs.ResponseDepartmentDTOS;

public class DepartmentDto : IDto
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public List<string>? EmployeesNames { get; set; }
}