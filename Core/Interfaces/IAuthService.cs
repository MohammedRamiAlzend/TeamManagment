using TMS.Core.Entities.Models;

namespace TMS.Core.Interfaces;

public interface IAuthService
{
    Task<DbRequest<User>> RegisterAsync(UserDto request );
    Task<TokenResponseDto?> LoginAsync(UserDto request );
    Task<User?> ValidateRefreshTokenAsync(Guid userId, string refreshToken);
    Task<TokenResponseDto?> RefreshTokenAsync(RefreshTokenRequestDto request);

    
}