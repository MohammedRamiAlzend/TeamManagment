using System.Text;

namespace TMS.Application.Handlers.CustomHandlers.DepartmentHandlers;

public class CreateDepartmentCommandHandler(IEntityCommiter commiter , ILogger<CreateDepartmentCommandHandler> logger) : IRequestHandler<CreateDepartmentCommand,ApiResponse>
{
    public async Task<ApiResponse> Handle(CreateDepartmentCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Create Department Command Handler is running");
        logger.LogInformation("Checking request informations ......");
        var validationResult = await ValidateDto(request.Dto);
        if (validationResult.IsSuccess is false)
        {
            return validationResult;
        }
        logger.LogInformation("informations Accepted ....");
        logger.LogInformation("checking if employee exists ...");
        var getEmployees = await GetEnrolledEmployeeByIdsAsListAsync(request.Dto.EnrolledEmployeeIds);
        if (getEmployees.IsSuccess is false)
        {
            return ApiResponse.Failure(code: HttpStatusCode.NotAcceptable, getEmployees.Message!);
        }
        logger.LogInformation("Employess was founded ....");
        logger.LogInformation("start creating department ....");

        var createDepartmentResult = await CreateDepartment(request.Dto, cancellationToken);
        
        return createDepartmentResult.IsSuccess 
            ? ApiResponse.Success(HttpStatusCode.OK,$"employee {request.Dto.Name} has been added successfully") 
            : ApiResponse.Failure(HttpStatusCode.BadRequest,createDepartmentResult.Message!);



    }

    private async Task<DbRequest<Department>> CreateDepartment(CreateDepartmentDto dto,CancellationToken token)
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
            Employees = getEmployees.Data??[],
            TeamLeaderId = dto.TeamLeaderId,
        };
        var addResult = await commiter.Departments.AddAsync(department);
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
        
        return addResult.IsSuccess ? DbRequest<Department>.Success(department) :
            DbRequest<Department>.Failure(addResult.Message!);
    }
    private async Task<ApiResponse<ICollection<Employee>>> GetEnrolledEmployeeByIdsAsListAsync(ICollection<int> dtoEnrolledEmployeeIds)
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

    private async Task<ApiResponse<CreatedDepartmentResponseDto>> ValidateDto(CreateDepartmentDto dto)
    {
        var errorBuilder = new StringBuilder();
        if (await commiter.Departments.AnyAsync(x =>
                x.Name.Equals(dto.Name, StringComparison.CurrentCultureIgnoreCase)))
        {
            errorBuilder.AppendLine($"{dto.Name} is already taken try another one");
        }
        if (await commiter.Departments.AnyAsync(
                x => x.ParentDepartmentId == dto.ParentDepartmentId,
                QueryIncludeHelper.IncludeDepartmentRelations()) is false)
        {
            errorBuilder.AppendLine($"department with {dto.ParentDepartmentId} was not found");
        }
        if (await commiter.Departments.AnyAsync(x =>
                x.Email.Equals(dto.Email, StringComparison.CurrentCultureIgnoreCase)))
        {
            errorBuilder.AppendLine($"{dto.Email} is already taken try another one");
        }       
        if (await commiter.Departments.AnyAsync(x =>
                x.PhoneNumber.Equals(dto.PhoneNumber, StringComparison.CurrentCultureIgnoreCase)))
        {
            errorBuilder.AppendLine($"{dto.PhoneNumber} is already taken try another one");
        }       
        
        if (await commiter.Employees.AnyAsync(x =>
                x.Id.Equals(dto.TeamLeaderId),
                QueryIncludeHelper.IncludeEmployeeRelations()) is false)
        {
            errorBuilder.AppendLine($"team leader with {dto.TeamLeaderId} was not found");
        }

        return errorBuilder.Length == 0
            ? ApiResponse<CreatedDepartmentResponseDto>.Failure(code: HttpStatusCode.NotAcceptable,
                errorBuilder.ToString())
            : ApiResponse<CreatedDepartmentResponseDto>.Success();
    }
}