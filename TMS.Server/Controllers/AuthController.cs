using Microsoft.AspNetCore.Authorization;
using TMS.Application.Commands.AuthCommands;
using TMS.Core;
using TMS.Core.Entities.Models;
using TMS.Core.Interfaces;


namespace TMS.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController(ISender sender,IAuthService authService) : ControllerBase
{
    [HttpPost("register")]
    public async Task<ApiResponse> Register(UserDto request)
    {
        return await sender.Send(new RegisterUserCommand(request));
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
