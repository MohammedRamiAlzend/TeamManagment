namespace TMS.Application.Services.Interfaces.DepartmentInterfaces;

public interface ICreateDepartmentService
{
    Task<DbRequest<Department>> CreateDepartment(CreateDepartmentDto dto, CancellationToken token);
} 