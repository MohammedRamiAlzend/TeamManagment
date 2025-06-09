namespace Contracts.CQRS.Commands.AuthCommands;

public record RegisterUserCommand(RegisterUserDto RegisterUserName) : IRequest<ApiResponse<User>>;