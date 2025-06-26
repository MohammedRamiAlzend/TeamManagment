using System.Net;
using TMS.Contract.CQRS.Commands.CustomCommands.DepartmentCommands.Dtos;

namespace TMS.Application.Services;

public class EmployeeService : IEmployeeService
{
    private readonly IEntityCommiter _commiter;
    public EmployeeService(IEntityCommiter commiter)
    {
        _commiter = commiter;
    }

    public async Task<DbRequest<Department>> UpdateDepartment(int departmentId, UpdateDepartmentDto dto, CancellationToken token)
    {
        var getDepartment = await _commiter.Departments.GetAsync(x => x.Id == departmentId,
            QueryIncludeHelper.IncludeDepartmentRelations());
        if (getDepartment.IsSuccess is false || getDepartment.Data is null)
        {
            return DbRequest<Department>.Failure(getDepartment.Message ?? $"department with {departmentId} was not found");
        }

        ICollection<Employee>? enrolledEmployees = null;
        if (dto.EnrolledEmployeeIds is not null)
        {
            var getEmployees = await GetEnrolledEmployeeByIdsAsCollectionAsync(dto.EnrolledEmployeeIds);
            if (getEmployees.IsSuccess is false)
            {
                return DbRequest<Department>.Failure(getEmployees.Message!);
            }
            enrolledEmployees = getEmployees.Data;
        }
        var departmentToUpdate = getDepartment.Data;

        departmentToUpdate.Name = dto.Name ?? departmentToUpdate.Name;
        departmentToUpdate.Email = dto.Email ?? departmentToUpdate.Email;
        departmentToUpdate.PhoneNumber = dto.PhoneNumber ?? departmentToUpdate.PhoneNumber;
        departmentToUpdate.ParentDepartmentId = dto.ParentDepartmentId ?? departmentToUpdate.ParentDepartmentId;
        if (enrolledEmployees is not null)
        {
            departmentToUpdate.Employees = enrolledEmployees;
        }
        departmentToUpdate.TeamLeaderId = dto.TeamLeaderId ?? departmentToUpdate.TeamLeaderId;

        var updateResult = await _commiter.Departments.UpdateAsync(departmentToUpdate);
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

        return updateResult.IsSuccess ? DbRequest<Department>.Success(departmentToUpdate, $"department {dto.Name} has been updated successfully") :
            DbRequest<Department>.Failure(updateResult.Message!);
    }

    private async Task<ApiResponse<ICollection<Employee>>> GetEnrolledEmployeeByIdsAsCollectionAsync(ICollection<int> dtoEnrolledEmployeeIds)
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