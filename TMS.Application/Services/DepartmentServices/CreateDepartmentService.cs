using TMS.Application.Services.Interfaces.DepartmentInterfaces;
using TMS.Contract.CQRS.Commands.CustomCommands.DepartmentCommands.Dtos;

namespace TMS.Application.Services.DepartmentServices;

public class CreateDepartmentService : ICreateDepartmentService
{
    private readonly IEntityCommiter _commiter;
    public CreateDepartmentService(IEntityCommiter commiter)
    {
        _commiter = commiter;
    }

    public async Task<DbRequest<Department>> CreateDepartment(CreateDepartmentDto dto, CancellationToken token)
    {
        var getEmployees = await GetEnrolledEmployeeByIdsAsListAsync(dto.EnrolledEmployeeIds);
        if (getEmployees.IsSuccess is false)
        {
            return DbRequest<Department>.Failure(getEmployees.Message!);
        }

        var department = new Department()
        {
            Name = dto.Name,
            Email = dto.Email,
            PhoneNumber = dto.PhoneNumber,
            ParentDepartmentId = dto.ParentDepartmentId,
            Employees = getEmployees.Data ?? [],
            TeamLeaderId = dto.TeamLeaderId,
        };
        var addResult = await _commiter.Departments.AddAsync(department);
        try
        {
            var commitResult = await _commiter.CommitAsync(token);
            if (commitResult == 0)
                return DbRequest<Department>.Failure("no changes has been made");
        }
        catch (Exception e)
        {
            return DbRequest<Department>.Failure(e.Message);
        }

        return addResult.IsSuccess ? DbRequest<Department>.Success(department) :
            DbRequest<Department>.Failure(addResult.Message!);
    }

    private async Task<ApiResponse<ICollection<Employee>>> GetEnrolledEmployeeByIdsAsListAsync(ICollection<int> dtoEnrolledEmployeeIds)
    {
        ICollection<Employee> employees = [];
        foreach (var id in dtoEnrolledEmployeeIds)
        {
            var dbResult = await _commiter.Employees.GetAsync(x => x.Id == id);
            if (dbResult.IsSuccess is false || dbResult.Data is null)
                return ApiResponse<ICollection<Employee>>.Failure(HttpStatusCode.NotFound, $"employee with {id} was not found");
            employees.Add(dbResult.Data);
        }
        return ApiResponse<ICollection<Employee>>.Success(employees);
    }
} 