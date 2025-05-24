using TMS.Core.Entities.Models;

namespace TMS.Application.Commands.AuthCommands;
public record RegisterUserCommand(UserDto UserName): IRequest<ApiResponse>;

public class RegisterUserCommandHandler(IAuthService authService) : IRequestHandler<RegisterUserCommand, ApiResponse>
{
    public async Task<ApiResponse> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var user = await authService.RegisterAsync(request.UserName);
        return user is null 
            ? ApiResponse.Failure(HttpStatusCode.BadRequest, "User already exists") 
            : ApiResponse.Success(user,HttpStatusCode.OK);
    }
}