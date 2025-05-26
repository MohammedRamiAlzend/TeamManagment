using TMS.Core.Entities.Models;

namespace TMS.Application.Commands.AuthCommands;
public record RegisterUserCommand(UserDto UserName): IRequest<ApiResponse<User>>;

public class RegisterUserCommandHandler(IAuthService authService) : IRequestHandler<RegisterUserCommand, ApiResponse<User>>
{
    public async Task<ApiResponse<User>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        return await authService.RegisterAsync(request.UserName);
    }
}