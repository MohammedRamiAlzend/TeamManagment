namespace TMS.Application.Handlers.CustomHandlers.AuthHandlers;

public class RegisterUserCommandHandler(IAuthService authService)
    : IRequestHandler<RegisterUserCommand, ApiResponse<User>>
{
    public async Task<ApiResponse<User>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        return await authService.RegisterAsync(request.RegisterUserName);
    }
}