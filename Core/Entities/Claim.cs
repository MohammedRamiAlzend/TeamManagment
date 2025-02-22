using TMS.Core.Entities.Interfaces;

namespace TMS.Core.Entities;
public class Claim : Entity, IAuditable, ISoftDeletable
{
    public int Id { get; set; }
    public string Name { get; set; }
    public ICollection<Role> RoleClaims { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
}


