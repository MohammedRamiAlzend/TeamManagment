
namespace TMS.Core.Entities;
public class Department : Entity
{
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
}
