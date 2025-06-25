using System.Text;
using Bogus.DataSets;

namespace TMS.Application.Handlers.CustomHandlers.DepartmentHandlers;

public class UpdateDepartmentCommandHandler(IEntityCommiter commiter, ILogger <UpdateDepartmentCommandHandler> logger)
    : IRequestHandler<UpdateDepartmentCommand, ApiResponse>
{
    public async Task<ApiResponse> Handle(UpdateDepartmentCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Update Department Command Handler is running");
        logger.LogInformation("Checking request informations ......");
        var validationResult = await ValidateDto(request.departmentId,request.Dto);
        if (validationResult.IsSuccess is false)
        {
            return validationResult;
        }
        logger.LogInformation("informations Accepted ....");
        
        logger.LogInformation("start Updating department ....");

        var updateDepartmentResult = await UpdateDepartment(request.departmentId,request.Dto, cancellationToken);
        
        return updateDepartmentResult.IsSuccess 
            ? ApiResponse.Success(HttpStatusCode.OK,$"department {request.Dto.Name} has been updated successfully") 
            : ApiResponse.Failure(HttpStatusCode.BadRequest,updateDepartmentResult.Message!);
    }

    private async Task<DbRequest<Department>> UpdateDepartment(int departmentId,UpdateDepartmentDto dto,CancellationToken token)
    {
        var getDepartment = await commiter.Departments.GetAsync(x=>x.Id == departmentId,
            QueryIncludeHelper.IncludeDepartmentRelations());
        if (getDepartment.IsSuccess is false || getDepartment.Data is null)
        {
            return DbRequest<Department>.Failure(getDepartment.Message??$"department with {departmentId} was not found");
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
        departmentToUpdate.Email = dto.Email?? departmentToUpdate.Email;
        departmentToUpdate.PhoneNumber = dto.PhoneNumber??departmentToUpdate.PhoneNumber;
        departmentToUpdate.ParentDepartmentId = dto.ParentDepartmentId ?? departmentToUpdate.ParentDepartmentId;
        if (enrolledEmployees is not null)
        {
            departmentToUpdate.Employees = enrolledEmployees;
        }
        departmentToUpdate.TeamLeaderId = dto.TeamLeaderId?? departmentToUpdate.TeamLeaderId;           
           
        var updateResult = await commiter.Departments.UpdateAsync(departmentToUpdate);
        try
        {
            var commitResult = await commiter.CommitAsync(token);
            if(commitResult == 0)
                return DbRequest<Department>.Failure("no changes has been made");
        }
        catch (Exception e)
        {
            return DbRequest<Department>.Failure(e.Message);
        }
        
        return updateResult.IsSuccess ? DbRequest<Department>.Success(departmentToUpdate,$"department {dto.Name} has been updated successfully") :
            DbRequest<Department>.Failure(updateResult.Message!);
    }
    private async Task<ApiResponse<ICollection<Employee>>> GetEnrolledEmployeeByIdsAsCollectionAsync(ICollection<int> dtoEnrolledEmployeeIds)
    {
        ICollection<Employee> employees = [];
        foreach (var id in dtoEnrolledEmployeeIds)
        {
            var dbResult = await commiter.Employees.GetAsync(x => x.Id == id);
            if(dbResult.IsSuccess is false || dbResult.Data is null)
                return ApiResponse<ICollection<Employee>>.Failure(HttpStatusCode.NotFound,$"employee with {id} was not found");
            employees.Add(dbResult.Data);
        }
        return ApiResponse<ICollection<Employee>>.Success(employees);
    }

    private async Task<ApiResponse> ValidateDto(int toUpdateId,UpdateDepartmentDto dto)
    {
        var errorBuilder = new StringBuilder();
        if (dto.Name is not null && await commiter.Departments.AnyAsync(x =>
                x.Name.ToLower() == (dto.Name).ToLower()))
        {
            errorBuilder.AppendLine($"{dto.Name} is already taken try another one");
        }
        if (dto.ParentDepartmentId  is not null && await commiter.Departments.AnyAsync(
                x => x.Id == dto.ParentDepartmentId,
                QueryIncludeHelper.IncludeDepartmentRelations()) is false)
        {
            errorBuilder.AppendLine($"department with {dto.ParentDepartmentId} was not found");
        }

        if (dto.ParentDepartmentId is not null && dto.ParentDepartmentId == toUpdateId)
        {
            errorBuilder.AppendLine($"parent department can not be the same as the department to update");
        }
        if ( dto.Email is not null && await commiter.Departments.AnyAsync(x =>
                x.Email.ToLower()== dto.Email.ToLower()))
        {
            errorBuilder.AppendLine($"{dto.Email} is already taken try another one");
        }       
        if (dto.PhoneNumber is not null && await commiter.Departments.AnyAsync(x =>
                x.PhoneNumber==dto.PhoneNumber))
        {
            errorBuilder.AppendLine($"{dto.PhoneNumber} is already taken try another one");
        }       
        
        if (dto.TeamLeaderId is not null && await commiter.Employees.AnyAsync(x =>
                x.Id==dto.TeamLeaderId,
                QueryIncludeHelper.IncludeEmployeeRelations()) is false)
        {
            errorBuilder.AppendLine($"team leader with {dto.TeamLeaderId} was not found");
        }
        
        if (dto.TeamLeaderId is not null && await commiter.Departments.AnyAsync(x =>
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
