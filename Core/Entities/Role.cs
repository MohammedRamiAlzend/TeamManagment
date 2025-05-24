namespace TMS.Core.Entities;

public class Role : Entity
{
    public string RoleName { get; set; }
    public ICollection<Permission> Permissions { get; set; }
    public ICollection<User> Users { get; set; }
}
