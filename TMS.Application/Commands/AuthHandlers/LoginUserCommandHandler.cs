using TMS.Core.CommunicationModels;

namespace TMS.Application.Commands.AuthHandlers;

public class LoginUserCommandHandler(IAuthService authService)
    : IRequestHandler<LoginUserCommand, ApiResponse<TokenResponseDto>>
{
    public async Task<ApiResponse<TokenResponseDto>> Handle(LoginUserCommand request,
        CancellationToken cancellationToken)
    {
        return await authService.LoginAsync(request.LoginUserDto);
    }
}