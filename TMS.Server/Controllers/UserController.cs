namespace TMS.Server.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize] 
public class AuthController(ISender sender) : ControllerBase
{
    [HttpPost("register")]
    [HasPermission(UserManagement.Register)]
    public async Task<ApiResponse<User>> Register([FromForm] RegisterUserDto request, CancellationToken token)
    {
        return await sender.Send(new RegisterUserCommand(request), token);
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<TokenResponseDto>>> Login(LoginUserDto request, CancellationToken token)
    {
        return await sender.Send(new LoginUserCommand(request), token);
    }

    [HttpPost("refresh-Token")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<TokenResponseDto>>> RefreshToken(
        RefreshTokenRequestDto request,
        CancellationToken token)
    {
        return await sender.Send(new RefreshTokenCommand(request), token);
    }

    [HttpGet]
    [HasPermission(UserManagement.Register)]
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