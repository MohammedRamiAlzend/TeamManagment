using TMS.Contract.CQRS.Commands.CustomCommands.ProjectCommands.Dtos;

namespace TMS.Application.Services;

public class ProjectValidator : IProjectValidator
{
    private readonly IEntityCommiter _commiter;
    public ProjectValidator(IEntityCommiter commiter)
    {
        _commiter = commiter;
    }

    public async Task<DbRequest> ValidateAdd(AddProjectDto dto)
    {
        var projectName = dto.ProjectName;
        var description = dto.Description;
        var startDate = dto.StartDate;
        var endDate = dto.EndDate;
        var enrolledMembersIds = dto.EnrolledMembersIds;
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
        if (startDate < DateTime.Now)
        {
            errorMessage += "Start date must be in the future\n";
        }
        if (await _commiter.Departments.AnyAsync(x => x.Id == departmentId) is false)
        {
            errorMessage += $"Department with id {departmentId} does not exist\n";
        }
        foreach (var id in enrolledMembersIds)
        {
            if ((await _commiter.Employees.AnyAsync(x => x.Id == id)) is false)
            {
                errorMessage += $"Employee with id {id} does not exist\n";
            }
        }
        return errorMessage.Length > 0 ? DbRequest.Failure(errorMessage) : DbRequest.Success();
    }
} 