namespace TMS.Contract.CQRS.Commands.CustomCommands.DepartmentCommands;

public record UpdateDepartmentTeamLeaderCommand(int DepartmentId , int TeamLeaderId) : IRequest<ApiResponse>;
