namespace TMS.Core.Entities;

public class Role : Entity
{
    public string RoleName { get; init; }
    public ICollection<Claim> RoleClaims { get; init; }
    public ICollection<Employee> EmployeesRoles { get; init; }
}
