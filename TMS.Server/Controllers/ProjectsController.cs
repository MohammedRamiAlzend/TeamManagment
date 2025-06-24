using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using TMS.Application.Handlers.CustomHandlers.ProjectHandlers;
using TMS.Contract.CQRS.Commands.CustomCommands.ProjectCommands;
using TMS.Contract.CQRS.Commands.CustomCommands.ProjectCommands.Dtos;
using TMS.Contract.CQRS.Commands.GenericCommands;
using TMS.Contract.CQRS.Queries.CustomQueries.ProjectQuries;
using TMS.Contract.CQRS.Queries.GenericQueries;

namespace TMS.Server.Controllers;
[ApiController]
[Route($"{ApiBase}/[controller]")]
[Authorize]
public class ProjectsController(ISender sender):ControllerBase
{
    [HttpGet(ProjectsEndPoint.GetAll)]
    [HasPermission(ProjectManagement.Get)]
    public async Task<ActionResult<ApiResponse<List<GetProjectResponse>>>> GetAllProjectsAsync(CancellationToken token)
    {
        return await sender.Send(new GetAllEntityQuery<Project, GetProjectResponse>(
            Include: QueryIncludeHelper.IncludeProjectRelations()
        ), token);
    }
    [HttpGet(ProjectsEndPoint.GetAllPaginated)]
    [HasPermission(ProjectManagement.Get)]
    public async Task<ActionResult<PaginatedApiResponse<GetProjectResponse>>> GetAllProjectsPaginatedAsync(
        [FromQuery]int pageSize,
        [FromQuery] int pageNumber,
        CancellationToken token)
    {
        return await sender.Send(new GetAllPaginatedEntityQuery<Project, GetProjectResponse>(
            PageSize: pageSize,
            PageNumber:pageNumber,
            Include: QueryIncludeHelper.IncludeProjectRelations()
        ), token);
    }
    [HttpGet(ProjectsEndPoint.Get)]
    [HasPermission(ProjectManagement.Get)]
    public async Task<ActionResult<ApiResponse<GetProjectResponse>>> GetProjectAsync(int projectId,CancellationToken token)
    {
        return await sender.Send(new GetEntityQuery<Project, GetProjectResponse>(
            Filter: x=> x.Id == projectId,
            Include: QueryIncludeHelper.IncludeProjectRelations()
        ), token);
    }
    [HttpPost(ProjectsEndPoint.Create)]
    [HasPermission(ProjectManagement.Add)]
    public async Task<ActionResult<ApiResponse<AddProjectDto>>> AddProjectAsync([FromForm] AddProjectDto project,
        CancellationToken token)
    {
        return await sender.Send(new AddProjectCommand(project), token);
    }
    [HttpPost(ProjectsEndPoint.AddTasks)]
    [HasPermission(ProjectManagement.Add)]
    public async Task<ActionResult<ApiResponse>> AddTasksToProjectAsync(
        [FromForm] AddTasksToProjectDto project,
        CancellationToken token)
    {
        return await sender.Send(new AddTasksToProjectCommand(project), token);
    }
    [HttpPut(ProjectsEndPoint.Update)]
    [HasPermission(ProjectManagement.Update)]
    public async Task<ActionResult<ApiResponse<UpdateProjectDto>>> UpdateProjectAsync(
        [FromRoute] int projectId,
        [FromBody] UpdateProjectDto project,
        CancellationToken token)
    {
        return await sender.Send(new UpdateProjectCommand(projectId,project), token);
    }
    [HttpDelete(ProjectsEndPoint.Delete)]
    [HasPermission(ProjectManagement.Delete)]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteProjectAsync(int projectId,
        CancellationToken token)
    {
        return await sender.Send(new DeleteProjectCommand(projectId), token);
    }
}