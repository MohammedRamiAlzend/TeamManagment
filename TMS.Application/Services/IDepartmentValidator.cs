namespace TMS.Application.Services;

public interface IDepartmentValidator
{
    Task<ApiResponse> ValidateUpdate(int departmentId, UpdateDepartmentDto dto);
} 