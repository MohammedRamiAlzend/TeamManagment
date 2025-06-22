using TMS.Contract.CQRS.Commands.CustomCommands.AuthCommands.Dtos;

namespace TMS.Contract.CQRS.Commands.CustomCommands.AuthCommands;

public record RefreshTokenCommand(RefreshTokenRequestDto refreshTokenRequest) : IRequest<ApiResponse<TokenResponseDto>>;