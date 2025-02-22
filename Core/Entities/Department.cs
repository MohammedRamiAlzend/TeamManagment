using TMS.Core.Entities.Interfaces;

namespace TMS.Core.Entities;
public class Department : Entity, IAuditable, ISoftDeletable
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }

    public int? ParentDepartmentID { get; set; }
    [ForeignKey("ParentDepartmentID")]
    public Department? ParentDepartment { get; set; }

    public int? TeamLeaderID { get; set; }
    [ForeignKey("TeamLeaderID")]
    public Employee? TeamLeader { get; set; }

    public ICollection<Department>? SubDepartments { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
}
