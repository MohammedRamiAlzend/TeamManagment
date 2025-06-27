using TMS.Contract.CQRS.Commands.CustomCommands.AuthCommands;
using TMS.Contract.CQRS.Commands.CustomCommands.AuthCommands.Dtos;
using Microsoft.Extensions.Logging;

namespace TMS.Server.Controllers;

/// <summary>
/// Controller for user authentication and management.
/// </summary>
[ApiController]
[Route("[controller]")]
[Authorize]
public class UserController : ControllerBase
{
    private readonly ISender _sender;
    private readonly ILogger<UserController> _logger;

    public UserController(ISender sender, ILogger<UserController> logger)
    {
        _sender = sender;
        _logger = logger;
    }

    /// <summary>
    /// Registers a new user.
    /// </summary>
    [HttpPost("register")]
    [HasPermission(UserManagement.Register)]
    [ProducesResponseType(typeof(ApiResponse<User>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ApiResponse<User>> Register([FromForm] RegisterUserDto request, CancellationToken token)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Invalid model state for Register");
            throw new BadHttpRequestException("Invalid model state");
        }
        try
        {
            return await _sender.Send(new RegisterUserCommand(request), token);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error registering user");
            throw;
        }
    }

    /// <summary>
    /// Logs in a user.
    /// </summary>
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<TokenResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<TokenResponseDto>>> Login([FromBody] LoginUserDto request, CancellationToken token)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Invalid model state for Login");
            return BadRequest(ModelState);
        }
        try
        {
            var result = await _sender.Send(new LoginUserCommand(request), token);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error logging in user");
            return StatusCode(500, "An error occurred while logging in.");
        }
    }

    /// <summary>
    /// Refreshes a user's token.
    /// </summary>
    [HttpPost("refresh-Token")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<TokenResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<TokenResponseDto>>> RefreshToken(
        [FromBody] RefreshTokenRequestDto request,
        CancellationToken token)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Invalid model state for RefreshToken");
            return BadRequest(ModelState);
        }
        try
        {
            var result = await _sender.Send(new RefreshTokenCommand(request), token);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error refreshing token");
            return StatusCode(500, "An error occurred while refreshing the token.");
        }
    }
}