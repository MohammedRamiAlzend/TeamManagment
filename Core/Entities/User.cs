namespace TMS.Core.Entities;

public class User
{
    public string UserName { get; set; } = string.Empty;
    public string HashedPassword { get; set; } = string.Empty;
}