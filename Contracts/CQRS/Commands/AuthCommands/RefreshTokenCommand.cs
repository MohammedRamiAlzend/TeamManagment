namespace Contracts.CQRS.Commands.AuthCommands;

public record RefreshTokenCommand(RefreshTokenRequestDto refreshTokenRequest) : IRequest<ApiResponse<TokenResponseDto>>;