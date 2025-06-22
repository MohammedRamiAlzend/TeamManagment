namespace TMS.Contract.Responses;

public class GetDepartmentResponse : IDto
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? TeamLeaderName { get; set; }
    public List<string>? EmployeesNames { get; set; }
}