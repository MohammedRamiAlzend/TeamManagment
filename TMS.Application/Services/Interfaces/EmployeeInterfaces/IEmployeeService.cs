namespace TMS.Application.Services.Interfaces.EmployeeInterfaces;

public interface IEmployeeService
{
    Task<DbRequest<Department>> UpdateDepartment(int departmentId, UpdateDepartmentDto dto, CancellationToken token);
} 