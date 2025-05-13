namespace TMS.Core.Entities;

public class Employee : Entity
{
    public string FirstName { get; init; }
    public string LastName { get; init; }
    public string? FatherName { get; init; }
    public string? MiddleName { get; init; }
    public string? MotherName { get; init; }
    public string NationalIdentificationNumber { get; init; }
    public DateTime BirthDate { get; init; }
    public DateTime HireDate { get; init; }
    public string? Phone { get; init; }
    public string? Email { get; init; }
    public string? Address { get; init; }
    public ICollection<Department> Departments { get; init; }
    public ICollection<Role> EmployeesRoles { get; init; }
    public ICollection<TaskAssignment>? AssignedToEmployeeTasks { get; init; }
    public ICollection<TaskAssignment>? AssignedByEmployeeTasks { get; init; }

}