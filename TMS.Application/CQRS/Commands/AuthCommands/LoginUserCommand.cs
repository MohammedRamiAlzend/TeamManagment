namespace TMS.Application.CQRS.Commands.AuthCommands;

public record LoginUserCommand(LoginUserDto LoginUserDto) : IRequest<ApiResponse<TokenResponseDto>>;