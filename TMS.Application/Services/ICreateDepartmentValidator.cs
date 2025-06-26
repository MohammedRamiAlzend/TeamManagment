namespace TMS.Application.Services;

public interface ICreateDepartmentValidator
{
    Task<ApiResponse<CreatedDepartmentResponseDto>> Validate(CreateDepartmentDto dto);
} 