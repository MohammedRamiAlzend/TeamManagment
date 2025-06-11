namespace TMS.Core.Entities;

public class Role : Entity
{
    public string Name { get; set; }
    public ICollection<Permission> Permissions { get; set; }
    public ICollection<User> Users { get; set; }
}