using TMS.Core.CommunicationModels;

namespace Contracts.CQRS.Commands.AuthCommands;

public record LoginUserCommand(LoginUserDto LoginUserDto) : IRequest<ApiResponse<TokenResponseDto>>;