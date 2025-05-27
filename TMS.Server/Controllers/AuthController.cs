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
    public async Task<ApiResponse> Register(RegisterUserDto request)
    {
        return await sender.Send(new RegisterUserCommand(request));
    }

    [HttpPost("login")]
    public async Task<ActionResult<ApiResponse<TokenResponseDto>>> Login(LoginUserDto request)
    {
        return await sender.Send(new LoginUserCommand(request));
    }

    [HttpPost("refresh-Token")]
    public async Task<ActionResult<ApiResponse<TokenResponseDto>>> RefreshToken(RefreshTokenRequestDto request)
    {
        return await sender.Send(new RefreshTokenCommand(request));
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
