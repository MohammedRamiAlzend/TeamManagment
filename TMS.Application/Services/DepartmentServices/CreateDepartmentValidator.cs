using System.Text;
using TMS.Application.Services.Interfaces.DepartmentInterfaces;
using TMS.Contract.CQRS.Commands.CustomCommands.DepartmentCommands.Dtos;

namespace TMS.Application.Services.DepartmentServices;

public class CreateDepartmentValidator : ICreateDepartmentValidator
{
    private readonly IEntityCommiter _commiter;
    public CreateDepartmentValidator(IEntityCommiter commiter)
    {
        _commiter = commiter;
    }

    public async Task<ApiResponse<CreatedDepartmentResponseDto>> Validate(CreateDepartmentDto dto)
    {
        var errorBuilder = new StringBuilder();
        if (await _commiter.Departments.AnyAsync(x =>
                x.Name == dto.Name.ToLower() ))
        {
            errorBuilder.AppendLine($"{dto.Name} is already taken try another one");
        }
        if (dto.ParentDepartmentId is not null && await _commiter.Departments.AnyAsync(
                x => x.Id == dto.ParentDepartmentId,
                QueryIncludeHelper.IncludeDepartmentRelations()) is false)
        {
            errorBuilder.AppendLine($"department with {dto.ParentDepartmentId} was not found");
        }
        if (await _commiter.Departments.AnyAsync(x =>
                x.Email.ToLower() == dto.Email.ToLower()))
        {
            errorBuilder.AppendLine($"{dto.Email} is already taken try another one");
        }       
        if (await _commiter.Departments.AnyAsync(x =>
                x.PhoneNumber.ToLower()== dto.PhoneNumber.ToLower()))
        {
            errorBuilder.AppendLine($"{dto.PhoneNumber} is already taken try another one");
        }      
        if (await _commiter.Departments.AnyAsync(x =>
                x.TeamLeaderId== dto.TeamLeaderId, QueryIncludeHelper.IncludeDepartmentRelations()))
        {
            errorBuilder.AppendLine($"{dto.TeamLeaderId} is already taken try another one");
        }  
        
        if (await _commiter.Employees.AnyAsync(x =>
                x.Id==dto.TeamLeaderId,
                QueryIncludeHelper.IncludeEmployeeRelations()) is false)
        {
            errorBuilder.AppendLine($"team leader with {dto.TeamLeaderId} was not found");
        }

        return errorBuilder.ToString().Length != 0 
            ? ApiResponse<CreatedDepartmentResponseDto>.Failure(code: HttpStatusCode.NotAcceptable,
                errorBuilder.ToString())
            : ApiResponse<CreatedDepartmentResponseDto>.Success();
    }
} 