using TMS.Application.CQRS.Commands.AuthCommands;

namespace TMS.Application.Handlers.AuthHandlers;

public class RegisterUserCommandHandler(IAuthService authService)
    : IRequestHandler<RegisterUserCommand, ApiResponse<User>>
{
    public async Task<ApiResponse<User>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        return await authService.RegisterAsync(request.RegisterUserName);
    }
}