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
    // [HttpPut(ProjectsEndPoint.Update)]
    // [HasPermission(ProjectManagement.Update)]
    // public async Task<ActionResult<ApiResponse<    
}