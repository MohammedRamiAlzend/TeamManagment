using TMS.Contract.CQRS.Commands.CustomCommands.AuthCommands.Dtos;

namespace TMS.Contract.CQRS.Commands.CustomCommands.AuthCommands;

public record RegisterUserCommand(RegisterUserDto RegisterUserName) : IRequest<ApiResponse<User>>;