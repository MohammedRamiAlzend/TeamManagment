namespace TMS.Core.Entities;
public class Permission : Entity
{
    public string Name { get; set; }
    public string? Description { get; set; } = string.Empty;
    public ICollection<Role> Roles { get; set; }
}
