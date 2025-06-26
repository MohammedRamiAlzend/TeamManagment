namespace TMS.Application.Services.Interfaces.DepartmentInterfaces;

public interface ICreateDepartmentValidator
{
    Task<ApiResponse<CreatedDepartmentResponseDto>> Validate(CreateDepartmentDto dto);
} 