namespace TMS.Core.Entities.Models;

public class RefreshTokenRequestDto
{
    public required Guid Id { get; set; }
    public required string RefreshToken { get; set; } = string.Empty;
}