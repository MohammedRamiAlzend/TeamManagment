namespace TMS.Application.Services.Interfaces.DepartmentInterfaces;

public interface IDepartmentValidator
{
    Task<ApiResponse> ValidateUpdate(int departmentId, UpdateDepartmentDto dto);
} 