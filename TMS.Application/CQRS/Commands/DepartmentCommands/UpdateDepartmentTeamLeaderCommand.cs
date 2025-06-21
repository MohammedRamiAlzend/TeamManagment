namespace TMS.Application.CQRS.Commands.DepartmentCommands;

public record UpdateDepartmentTeamLeaderCommand(int DepartmentId , int TeamLeaderId) : IRequest<ApiResponse>;
