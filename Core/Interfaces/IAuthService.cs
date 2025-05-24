using TMS.Core.Entities.Models;

namespace TMS.Core.Interfaces;

public interface IAuthService
{
    Task<User?> RegisterAsync(UserDto request );
    Task<TokenResponseDto?> LoginAsync(UserDto request );
    Task<User?> ValidateRefreshTokenAsync(Guid userId, string refreshToken);
    Task<TokenResponseDto?> RefreshTokenAsync(RefreshTokenRequestDto request);

    
}