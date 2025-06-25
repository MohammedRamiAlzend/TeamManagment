namespace TMS.Contract.CQRS.Commands.CustomCommands.DepartmentCommands;

public record DeleteDepartmentCommand (int departmentId): IRequest<ApiResponse>;

