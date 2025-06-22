namespace TMS.Contract.Entities;

public class Department : Entity
{
    public string Name { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }

    public int? ParentDepartmentId { get; set; }
    public Department? ParentDepartment { get; set; }

    public int? TeamLeaderId { get; set; }
    public Employee TeamLeader { get; set; }

    public ICollection<Department>? SubDepartments { get; init; }
    public ICollection<Employee> Employees { get; set; }
}