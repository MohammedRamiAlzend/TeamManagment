using System.Security.Claims;

namespace Core.Data.Entities;
public class Claim : IHasId, IAuditable, ISoftDeletable
{
    public int Id { get; set; }
    public string Name { get; set; }
    public ICollection<Role> RoleClaims { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
}


