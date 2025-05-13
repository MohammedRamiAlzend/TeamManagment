
namespace TMS.Core.Entities;
public class Department : Entity
{
    public string Name { get; init; }
    public string? Email { get; init; }
    public string? PhoneNumber { get; init; }

    public int? ParentDepartmentID { get; init; }
    [ForeignKey("ParentDepartmentID")]
    public Department? ParentDepartment { get; init; }

    public int? TeamLeaderID { get; init; }
    [ForeignKey("TeamLeaderID")]
    public Employee? TeamLeader { get; init; }

    public ICollection<Department>? SubDepartments { get; init; }
}
