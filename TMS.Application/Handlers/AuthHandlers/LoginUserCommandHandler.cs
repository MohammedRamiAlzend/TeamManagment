using TMS.Contract.CQRS.Commands.CustomCommands.AuthCommands;
using TMS.Contract.CQRS.Commands.CustomCommands.AuthCommands.Dtos;

namespace TMS.Application.Handlers.AuthHandlers;

public class LoginUserCommandHandler(IAuthService authService)
    : IRequestHandler<LoginUserCommand, ApiResponse<TokenResponseDto>>
{
    public async Task<ApiResponse<TokenResponseDto>> Handle(LoginUserCommand request,
        CancellationToken cancellationToken)
    {
        return await authService.LoginAsync(request.LoginUserDto);
    }
}