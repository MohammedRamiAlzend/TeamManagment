namespace TMS.Application.CQRS.Commands.AuthCommands;

public record RegisterUserCommand(RegisterUserDto RegisterUserName) : IRequest<ApiResponse<User>>;