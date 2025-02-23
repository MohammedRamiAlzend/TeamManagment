
namespace TMS.Core.Entities;
public class Claim : Entity
{
    public string Name { get; set; }
    public ICollection<Role> RoleClaims { get; set; }
}


