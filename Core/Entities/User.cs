namespace TMS.Core.Entities;

public class User
{
    public Guid Id { get; set; }
    public string UserName { get; set; }
    public string PasswordHash { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiryTime { get; set; }

    public int EmployeeId { get; set; }
    public Employee Employee { get; set; }
    public ICollection<Role> Roles { get; set; }
}