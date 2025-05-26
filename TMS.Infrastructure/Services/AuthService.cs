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

public class AuthService(AppDbContext context, IEntityCommiter commiter, IConfiguration configuration) : IAuthService
{
    public async Task<DbRequest<User>> RegisterAsync(UserDto request)
    {
        if (string.IsNullOrWhiteSpace(request.UserName) || string.IsNullOrWhiteSpace(request.Password))
            return DbRequest<User>.Failure("Username and password are required.");

        if (await context.Users.AnyAsync(x => x.UserName.ToLower() == request.UserName.ToLower()))
            return DbRequest<User>.Failure("User already exists");

        var user = new User();
        var hashedPassword = new PasswordHasher<User>().HashPassword(user, request.Password);
        user.UserName = request.UserName;
        user.PasswordHash = hashedPassword;
        user.RefreshToken = null;
        
        var getDepartmentsRequest = await GetDepartmentsByIdAsync(request.DepartmentIds);
        var departmentsList = getDepartmentsRequest.Data ?? [];
        
        user.Employee = new Employee()
        {
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Phone = request.Phone,
            BirthDate = request.BirthDate,
            HireDate = request.HireDate,
            NationalIdentificationNumber = request.NationalIdentificationNumber,
            Departments = departmentsList
        };

        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            return DbRequest<User>.Failure("An error occurred while registering the user.");
        }

        return DbRequest<User>.Success(user, "User registered successfully");
    }

    private async Task<DbRequest<List<Department>>> GetDepartmentsByIdAsync(ICollection<int>? departmentIds)
    {
        if (departmentIds is null)
        {
            return DbRequest<List<Department>>.Failure();
        }
        return await commiter.Departments.GetAllAsync(
            filter: department => departmentIds.Contains(department.Id)
        );
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
        var rolesAndPermissionsString
            = BuildRolesAndPermissionsString(user);
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name,user.UserName) ,
            new Claim(ClaimTypes.NameIdentifier,user.Id.ToString()),
            new Claim(ClaimTypes.Role,rolesAndPermissionsString
            )
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
    private static string BuildRolesAndPermissionsString(User user)
    {
        var roles = user.Roles.Select(x => x.Name);
        var permissions = user.Roles.SelectMany(x => x.Permissions).Select(x => x.Name);
        return string.Join(",", roles.Concat(permissions));
    }

    private static string GenerateRefreshToken()
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