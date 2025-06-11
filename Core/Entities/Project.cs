namespace TMS.Core.Entities;

public class Project : Entity
{
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Status { get; set; }
    public int DepartmentId { get; set; }
    public Department Department { get; set; }

    public ICollection<Employee> TeamMembers { get; set; }
    public ICollection<WorkTask> Tasks { get; set; }
}