namespace TMS.Contract.Entities;

public class Permission : Entity
{
    public string Name { get; set; }
    public ICollection<Role> Roles { get; set; }
}