namespace TMS.Contract.CQRS.Commands.AuthCommands;

public record LoginUserCommand(LoginUserDto LoginUserDto) : IRequest<ApiResponse<TokenResponseDto>>;