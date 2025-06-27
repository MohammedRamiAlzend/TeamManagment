namespace TMS.Contract.CQRS.Queries.CustomQueries.DepartmentQuries;

public class GetDepartmentResponse : IDto
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public int TeamLeaderId { get; set; }
    public string? TeamLeaderName { get; set; }
    public Dictionary<int,string>? EmployeesNamesAsDictionary { get; set; }
}