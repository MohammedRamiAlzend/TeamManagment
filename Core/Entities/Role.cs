using TMS.Core.Entities.Interfaces;

namespace TMS.Core.Entities
{
    public class Role : IHasId, IAuditable, ISoftDeletable
    {
        public int Id { get; set; }
        public string RoleName { get; set; }
        public ICollection<Claim> RoleClaims { get; set; }
        public ICollection<Employee> EmployeesRoles { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
    }

}
