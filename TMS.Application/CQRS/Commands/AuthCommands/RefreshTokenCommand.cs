namespace TMS.Application.CQRS.Commands.AuthCommands;

public record RefreshTokenCommand(RefreshTokenRequestDto refreshTokenRequest) : IRequest<ApiResponse<TokenResponseDto>>;