using TMS.Core.Entities.Models;
using TMS.Core.CommunicationModels;

namespace TMS.Core.Interfaces;

public interface IAuthService
{
    Task<DbRequest<User>> RegisterAsync(RegisterUserDto request);
    Task<ApiResponse<TokenResponseDto>> LoginAsync(LoginUserDto request);
    Task<User?> ValidateRefreshTokenAsync(Guid userId, string refreshToken);
    Task<TokenResponseDto?> RefreshTokenAsync(RefreshTokenRequestDto request);
}