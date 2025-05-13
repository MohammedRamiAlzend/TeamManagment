
namespace TMS.Core.Entities;
public class Claim : Entity
{
    public string Name { get; init; }
    public ICollection<Role> RoleClaims { get; init; }
}


