namespace TMS.Contract.CQRS.Commands.CustomCommands.AuthCommands.Dtos;

public class RefreshTokenRequestDto
{
    public required Guid Id { get; set; }
    public required string RefreshToken { get; set; } = string.Empty;
}