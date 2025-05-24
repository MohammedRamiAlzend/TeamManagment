
namespace TMS.Core.Entities;
public class Permission : Entity
{
    public string Name { get; init; }
    public ICollection<Role> Roles { get; set; }
}


