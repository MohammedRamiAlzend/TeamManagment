using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using TMS.Core.Entities;
using TMS.Core.Entities.Models;
using TMS.Infrastructure.Data.DbContextTools;

namespace TMS.Infrastructure.Services;

public class AuthService(AppDbContext context, IConfiguration configuration) : IAuthService
{
    public async Task<User?> RegisterAsync(UserDto request)
    {
        if (await context.Users.AnyAsync(x => x.UserName == request.UserName))
        {
            return null;
        }
        
        var user = new User();
        var hashedPassword = new PasswordHasher<User>()
            .HashPassword(user, request.Password);
        user.UserName = request.UserName;
        user.PasswordHash = hashedPassword;

        await context.Users.AddAsync(user);
        await context.SaveChangesAsync();
        return user;
    }

    public async Task<TokenResponseDto?> LoginAsync(UserDto request)
    {
        var user = await context.Users.FirstOrDefaultAsync(x=>x.UserName == request.UserName);
        if (user is null)
            return null;
        
        if (new PasswordHasher<User>().VerifyHashedPassword(user, user.PasswordHash, request.Password)
            == PasswordVerificationResult.Failed)
        {
            return null;
        }
        return await GenerateTokenResponse(user);
    }

    private async Task<TokenResponseDto> GenerateTokenResponse(User user)
    {
        return new TokenResponseDto
        {
            AccessToken = CreateToken(user),
            RefreshToken = await GenerateAndSaveRefreshToken(user)
        };
    }

    public async Task<TokenResponseDto?> RefreshTokenAsync(RefreshTokenRequestDto request)
    {
        var user = await ValidateRefreshTokenAsync(request.Id, request.RefreshToken);
        if(user is null)
        {
            return null;
        }
        return await GenerateTokenResponse(user);
    }

    private string CreateToken(User user)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name,user.UserName) ,
            new Claim(ClaimTypes.NameIdentifier,user.Id.ToString()),
            // new Claim(ClaimTypes.Role,user.Role)
        };
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(configuration["AppSettings:Token"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);
        var tokenDescriptor = new JwtSecurityToken(
            issuer: configuration["AppSettings:Issuer"],
            audience: configuration["AppSettings:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddDays(1),
            signingCredentials: creds
        );
        return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
    }

    private string GenerateRefreshToken()
    {
        var randomNumner = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        return Convert.ToBase64String(randomNumner);
    }


    public async Task<User?> ValidateRefreshTokenAsync(Guid userId,string refreshToken)
    {
        var user =  await context.Users.FindAsync(userId);
        if (user is null 
            || user.RefreshToken != refreshToken 
            || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
        {
            return null;
        }
        return user;
    }

    private async Task<string> GenerateAndSaveRefreshToken(User user)
    {
        var refreshToken = GenerateRefreshToken();
        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
        await context.SaveChangesAsync();
        return refreshToken;
    }
}