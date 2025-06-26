namespace TMS.Application.Services;

public interface IEmployeeService
{
    Task<DbRequest<Department>> UpdateDepartment(int departmentId, UpdateDepartmentDto dto, CancellationToken token);
} 