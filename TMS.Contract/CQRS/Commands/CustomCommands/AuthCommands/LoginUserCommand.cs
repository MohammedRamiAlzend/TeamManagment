using TMS.Contract.CQRS.Commands.CustomCommands.AuthCommands.Dtos;

namespace TMS.Contract.CQRS.Commands.CustomCommands.AuthCommands;

public record LoginUserCommand(LoginUserDto LoginUserDto) : IRequest<ApiResponse<TokenResponseDto>>;