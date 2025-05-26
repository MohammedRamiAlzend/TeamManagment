namespace TMS.Core.Entities;

public class Employee : Entity
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string NationalIdentificationNumber { get; set; }
    public DateTime BirthDate { get; set; }
    public DateTime HireDate { get; set; }
    public string Phone { get; set; }
    public string Email { get; set; }
    
    
    public User User { get; set; }
    public ICollection<Department> Departments { get; set; }
    public ICollection<Project> Projects { get; set; }
    public ICollection<WorkTask> CreatedTasks { get; set; }
    public ICollection<WorkTask> AssignedTasks { get; set; }

}