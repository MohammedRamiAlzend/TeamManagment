namespace TMS.Core.Entities;

public class Role : Entity
{
    public string RoleName { get; set; }
    public ICollection<Claim> RoleClaims { get; set; }
    public ICollection<Employee> EmployeesRoles { get; set; }
}
