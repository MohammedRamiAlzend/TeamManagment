using TMS.Contract.CQRS.Commands.CustomCommands.DepartmentCommands.Dtos;

namespace TMS.Contract.CQRS.Commands.CustomCommands.DepartmentCommands;

public record UpdateDepartmentCommand(int departmentId,UpdateDepartmentDto Dto) : IRequest<ApiResponse>;
