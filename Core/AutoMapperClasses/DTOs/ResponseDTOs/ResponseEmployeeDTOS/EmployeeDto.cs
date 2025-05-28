using TMS.Core.AutoMapperClasses.DTOs.ResponseDTOs.ResponseDepartmentDTOS;

namespace TMS.Core.AutoMapperClasses.DTOs.ResponseDTOs.ResponseEmployeeDTOS;
    public class EmployeeDto : IDto
    {
        public int Id { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public ICollection<DepartmentDto>? Departments { get; set; }
    }
  

