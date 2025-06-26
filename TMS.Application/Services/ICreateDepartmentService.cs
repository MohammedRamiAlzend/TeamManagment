namespace TMS.Application.Services;

public interface ICreateDepartmentService
{
    Task<DbRequest<Department>> CreateDepartment(CreateDepartmentDto dto, CancellationToken token);
} 