using TMS.Contract.CQRS.Commands.CustomCommands.ProjectCommands;
using TMS.Contract.CQRS.Commands.CustomCommands.ProjectCommands.Dtos;
using TMS.Contract.Entities.Enums;
using TMS.Application.Services.Interfaces.ProjectInterfaces;

namespace TMS.Application.Handlers.CustomHandlers.ProjectHandlers;

public class AddProjectHandler : IRequestHandler<AddProjectCommand, ApiResponse<AddProjectDto>>
{
    private readonly IProjectValidator _validator;
    private readonly IProjectService _projectService;
    private readonly IEntityCommiter _commiter;

    public AddProjectHandler(IProjectValidator validator, IProjectService projectService, IEntityCommiter commiter)
    {
        _validator = validator;
        _projectService = projectService;
        _commiter = commiter;
    }

    public async Task<ApiResponse<AddProjectDto>> Handle(AddProjectCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAdd(request.EntityDto);
        if (!validationResult.IsSuccess)
            return ApiResponse<AddProjectDto>.Failure(HttpStatusCode.BadRequest, validationResult.Message!);

        var getEnrolledEmployees = await _projectService.GetEnrolledMembers(request.EntityDto.EnrolledMembersIds);
        var getTasksForProject = await _projectService.GetTasksForProject(request.EntityDto.GuidTasks);
        Project project = new()
        {
            DepartmentId = request.EntityDto.DepartmentId,
            Description = request.EntityDto.Description,
            StartDate = request.EntityDto.StartDate,
            EndDate = request.EntityDto.EndDate,
            Name = request.EntityDto.ProjectName,
            TeamMembers = getEnrolledEmployees,
            Tasks = getTasksForProject,
            Status = nameof(ProjectStatus.Pending)
        };
        var addResult = await _commiter.Projects.AddAsync(project);
        var commitResult = await _commiter.CommitAsync(cancellationToken);
        request.EntityDto.Id = project.Id;
        return addResult.IsSuccess is false || commitResult == 0
            ? ApiResponse<AddProjectDto>.Failure(HttpStatusCode.BadRequest, addResult.Message!)
            : ApiResponse<AddProjectDto>.Success(request.EntityDto);
    }
}