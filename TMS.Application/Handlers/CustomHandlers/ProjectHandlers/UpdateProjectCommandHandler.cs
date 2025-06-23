using TMS.Contract.CQRS.Commands.CustomCommands.ProjectCommands;
using TMS.Contract.CQRS.Commands.CustomCommands.ProjectCommands.Dtos;
using TMS.Contract.Entities.Enums;

namespace TMS.Application.Handlers.CustomHandlers.ProjectHandlers;

public class UpdateProjectCommandHandler(IEntityCommiter commiter) : IRequestHandler<UpdateProjectCommand, ApiResponse<UpdateProjectDto>>
{
    public async Task<ApiResponse<UpdateProjectDto>> Handle(UpdateProjectCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await ValidateDto(request.Project);
        if (validationResult.IsSuccess is false)
            return ApiResponse<UpdateProjectDto>.Failure(HttpStatusCode.BadRequest, validationResult.Message);

        var projectToUpdate = await commiter.Projects.GetAsync(p => p.Id == request.Project.Id);
        if (projectToUpdate.IsSuccess is false || projectToUpdate.Data is null)
            return ApiResponse<UpdateProjectDto>.Failure(HttpStatusCode.NotFound, "Project not found.");

        projectToUpdate.Data.Name = request.Project.Name;
        projectToUpdate.Data.Description = request.Project.Description;
        projectToUpdate.Data.StartDate = request.Project.StartDate;
        projectToUpdate.Data.EndDate = request.Project.EndDate;
        projectToUpdate.Data.Status = request.Project.ProjectStatus.ToString();
        projectToUpdate.Data.DepartmentId = request.Project.DepartmentId;
        // Potentially update team members if that's part of UpdateProjectDto

        var updateResult = await commiter.Projects.UpdateAsync(projectToUpdate.Data);
        var commitResult = await commiter.CommitAsync(cancellationToken);

        return updateResult.IsSuccess is false || commitResult == 0
            ? ApiResponse<UpdateProjectDto>.Failure(HttpStatusCode.BadRequest, updateResult.Message)
            : ApiResponse<UpdateProjectDto>.Success(request.Project);
    }

    private async Task<DbRequest> ValidateDto(UpdateProjectDto dto)
    {
        var errorMessage = "";
        if (dto.Name.Length < 3)
        {
            errorMessage += "Project name must be at least 3 characters long\n";
        }
        if (dto.Description.Length < 3)
        {
            errorMessage += "Project description must be at least 3 characters long\n";
        }
        if (dto.StartDate > dto.EndDate)
        {
            errorMessage += "Start date must be before end date\n";
        }
        // Note: For update, startDate < DateTime.Now might not be an error if updating an ongoing project.
        // Consider your business logic for this validation.

        if (await commiter.Departments.AnyAsync(x => x.Id == dto.DepartmentId) is false)
        {
            errorMessage += $"Department with id {dto.DepartmentId} does not exist\n";
        }

        // Add more specific validation for project status, etc. if needed

        return errorMessage.Length > 0 ? DbRequest.Failure(errorMessage) : DbRequest.Success();
    }
} 