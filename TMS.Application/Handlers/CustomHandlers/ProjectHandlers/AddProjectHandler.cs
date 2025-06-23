using TMS.Contract.CQRS.Commands.CustomCommands.ProjectCommands.Dtos;
using TMS.Contract.Entities.Enums;

namespace TMS.Application.Handlers.CustomHandlers.ProjectHandlers;
public record AddProjectCommand(AddProjectDto EntityDto):IRequest<ApiResponse<AddProjectDto>>;

public class AddProjectHandler(IEntityCommiter commiter) : IRequestHandler<AddProjectCommand,ApiResponse<AddProjectDto>>
{
    public async Task<ApiResponse<AddProjectDto>> Handle(AddProjectCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await ValidateDto(request.EntityDto);
        if(validationResult.IsSuccess is false)
            return ApiResponse<AddProjectDto>.Failure(HttpStatusCode.BadRequest, validationResult.Message);
        
        var getEnrolledEmployees = await GetEnrolledMembers(request.EntityDto.EnrolledMembersIds);

        Project project = new()
        {
            DepartmentId = request.EntityDto.DepartmentId,
            Description = request.EntityDto.Description,
            StartDate = request.EntityDto.StartDate,
            EndDate = request.EntityDto.EndDate,
            Name = request.EntityDto.ProjectName,
            TeamMembers = getEnrolledEmployees,
            Status = nameof(ProjectStatus.Pending)
        };
        var addResult =await commiter.Projects.AddAsync(project);
        var commitResult = await commiter.CommitAsync(cancellationToken);
        return addResult.IsSuccess is false  || commitResult == 0
            ? ApiResponse<AddProjectDto>.Failure(HttpStatusCode.BadRequest, addResult.Message) 
            : ApiResponse<AddProjectDto>.Success(request.EntityDto);
    }

    private async Task<DbRequest> ValidateDto(AddProjectDto dto)
    {
        var projectName = dto.ProjectName;
        var description = dto.Description;
        var startDate = dto.StartDate;
        var endDate = dto.EndDate;
        var enrolledMembersIds =dto.EnrolledMembersIds;
        var departmentId = dto.DepartmentId;
        var errorMessage = "";
        if (projectName.Length < 3)
        {
            errorMessage += "Project name must be at least 3 characters long\n";
        }
        if (description.Length < 3)
        {
            errorMessage += "Project description must be at least 3 characters long\n";
        }
        if (startDate > endDate)
        {
            errorMessage += "Start date must be before end date\n";
        }
        if(startDate < DateTime.Now)
        {
            errorMessage += "Start date must be in the future\n";
        }

        if (await commiter.Departments.AnyAsync(x => x.Id == departmentId) is false)
        {
            errorMessage += $"Department with id {departmentId} does not exist\n";       
        }
        foreach (var id in enrolledMembersIds)
        {
            if ((await commiter.Employees.AnyAsync(x => x.Id == id)) is false)
            {
                errorMessage += $"Employee with id {id} does not exist\n";
            }
        }
        
        return errorMessage.Length > 0 ? DbRequest.Failure(errorMessage) : DbRequest.Success();
    }

    private async Task<List<Employee>> GetEnrolledMembers(List<int> enrolledMembersIds)
    {
        var employees = new List<Employee>();
        foreach (var id in enrolledMembersIds)
        {
            employees.Add((await commiter.Employees.GetAsync(x => x.Id == id)).Data!);
        }

        return employees;
    }

 
}