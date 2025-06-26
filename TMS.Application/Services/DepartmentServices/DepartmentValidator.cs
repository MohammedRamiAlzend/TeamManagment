using System.Text;
using System.Net;
using TMS.Contract.CQRS.Commands.CustomCommands.DepartmentCommands.Dtos;
using TMS.Application.Services.Interfaces.DepartmentInterfaces;

namespace TMS.Application.Services.DepartmentServices;

public class DepartmentValidator : IDepartmentValidator
{
    private readonly IEntityCommiter _commiter;
    public DepartmentValidator(IEntityCommiter commiter)
    {
        _commiter = commiter;
    }

    public async Task<ApiResponse> ValidateUpdate(int toUpdateId, UpdateDepartmentDto dto)
    {
        var errorBuilder = new StringBuilder();
        if (dto.Name is not null && await _commiter.Departments.AnyAsync(x =>
                x.Name.ToLower() == dto.Name.ToLower()))
        {
            errorBuilder.AppendLine($"{dto.Name} is already taken try another one");
        }
        if (dto.ParentDepartmentId  is not null && await _commiter.Departments.AnyAsync(
                x => x.Id == dto.ParentDepartmentId,
                QueryIncludeHelper.IncludeDepartmentRelations()) is false)
        {
            errorBuilder.AppendLine($"department with {dto.ParentDepartmentId} was not found");
        }

        if (dto.ParentDepartmentId is not null && dto.ParentDepartmentId == toUpdateId)
        {
            errorBuilder.AppendLine($"parent department can not be the same as the department to update");
        }
        if ( dto.Email is not null && await _commiter.Departments.AnyAsync(x =>
                x.Email.ToLower()== dto.Email.ToLower()))
        {
            errorBuilder.AppendLine($"{dto.Email} is already taken try another one");
        }       
        if (dto.PhoneNumber is not null && await _commiter.Departments.AnyAsync(x =>
                x.PhoneNumber==dto.PhoneNumber))
        {
            errorBuilder.AppendLine($"{dto.PhoneNumber} is already taken try another one");
        }       
        
        if (dto.TeamLeaderId is not null && await _commiter.Employees.AnyAsync(x =>
                x.Id==dto.TeamLeaderId,
                QueryIncludeHelper.IncludeEmployeeRelations()) is false)
        {
            errorBuilder.AppendLine($"team leader with {dto.TeamLeaderId} was not found");
        }
        
        if (dto.TeamLeaderId is not null && await _commiter.Departments.AnyAsync(x =>
                    x.TeamLeaderId==dto.TeamLeaderId,
                QueryIncludeHelper.IncludeDepartmentRelations()))
        {
            errorBuilder.AppendLine($"team leader with {dto.TeamLeaderId} was already taken for another department");
        }
        return errorBuilder.ToString().Length != 0
            ? ApiResponse.Failure(code: HttpStatusCode.NotAcceptable,
                errorBuilder.ToString())
            : ApiResponse.Success();
    }
} 