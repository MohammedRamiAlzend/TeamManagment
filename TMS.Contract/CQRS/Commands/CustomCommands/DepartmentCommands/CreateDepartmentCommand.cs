using TMS.Contract.CQRS.Commands.CustomCommands.DepartmentCommands.Dtos;

namespace TMS.Contract.CQRS.Commands.CustomCommands.DepartmentCommands;

public record CreateDepartmentCommand(CreateDepartmentDto Dto) : IRequest<ApiResponse>;
