namespace TMS.Contract.CQRS.Queries.CustomQueries.DepartmentQuries;

public class GetDepartmentResponse : IDto
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? TeamLeaderName { get; set; }
    public List<string>? EmployeesNames { get; set; }
}