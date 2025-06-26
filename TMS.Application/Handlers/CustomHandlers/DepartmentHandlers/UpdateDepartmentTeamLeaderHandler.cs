using TMS.Application.Services;
namespace TMS.Application.Handlers.CustomHandlers.DepartmentHandlers;

public class UpdateDepartmentTeamLeaderHandler : IRequestHandler<UpdateDepartmentTeamLeaderCommand, ApiResponse>
{
    private readonly IUpdateDepartmentTeamLeaderService _service;
    private readonly ILogger<UpdateDepartmentTeamLeaderHandler> _logger;

    public UpdateDepartmentTeamLeaderHandler(IUpdateDepartmentTeamLeaderService service, ILogger<UpdateDepartmentTeamLeaderHandler> logger)
    {
        _service = service;
        _logger = logger;
    }

    public async Task<ApiResponse> Handle(UpdateDepartmentTeamLeaderCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("UpdateDepartmentTeamLeaderHandler is running");
        return await _service.UpdateTeamLeader(request.DepartmentId, request.TeamLeaderId, cancellationToken);
    }
}