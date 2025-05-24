using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using TMS.Core.Entities.Models;
using TMS.Core.Interfaces;


namespace TMS.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("register")]
    public async Task<ActionResult<User>> Register(UserDto request)
    {
        var user = await authService.RegisterAsync(request);
        if (user is null)
        {
            return BadRequest("User already exists");
        }

        return Ok(user);
    }

    [HttpPost("login")]
    public async Task<ActionResult<TokenResponseDto>> Login(UserDto request)
    {
        var result = await authService.LoginAsync(request);
        if (result is null)
        {
            return BadRequest("Invalid username or password");
        }

        return Ok(result);
    }

    [HttpPost("refresh-Token")]
    public async Task<ActionResult<TokenResponseDto>> RefreshToken(RefreshTokenRequestDto request)
    {
        var result = await authService.RefreshTokenAsync(request);
        if (result is null || result.AccessToken is null || result.RefreshToken is null)
        {
            return BadRequest("Invalid refresh token");
        }

        return Ok(result);
    }

    [HttpGet]
    [Authorize]
    public IActionResult AuthenticatedOnlyEndpoint()
    {
        return Ok("You Are Authenticated");
    }

    [HttpGet("admin-only")]
    [Authorize(Roles = "Admin")]
    public IActionResult AdminOnlyEndPoint()
    {
        return Ok("You Are Authenticated");
    }
}
