using TMS.Application.CQRS.Commands.AuthCommands;

namespace TMS.Application.Handlers.AuthHandlers;

public class RefreshTokenCommandHandler(IAuthService authService)
    : IRequestHandler<RefreshTokenCommand, ApiResponse<TokenResponseDto>>
{
    public async Task<ApiResponse<TokenResponseDto>> Handle(RefreshTokenCommand request,
        CancellationToken cancellationToken)
    {
        var result = await authService.RefreshTokenAsync(request.refreshTokenRequest);
        if (result?.AccessToken is null || result.RefreshToken is null)
            return ApiResponse<TokenResponseDto>.Failure(HttpStatusCode.BadRequest, "Invalid refresh token");

        return ApiResponse<TokenResponseDto>.Success(result);
    }
}