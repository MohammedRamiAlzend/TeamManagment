using TMS.Core.Entities.Models;

namespace TMS.Application.Commands.AuthCommands;

public record LoginUserCommand(LoginUserDto LoginUserDto) : IRequest<ApiResponse<TokenResponseDto>>;

public class LoginUserCommandHandler(IAuthService authService) : IRequestHandler<LoginUserCommand,ApiResponse< TokenResponseDto>>
{
    public async Task<ApiResponse<TokenResponseDto>> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
       return await authService.LoginAsync(request.LoginUserDto);
    }
}
