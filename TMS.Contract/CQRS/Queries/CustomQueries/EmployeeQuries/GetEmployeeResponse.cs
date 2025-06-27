namespace TMS.Contract.CQRS.Queries.CustomQueries.EmployeeQuries;

public class GetEmployeeResponse : IDto
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string NationalIdentificationNumber { get; set; } = string.Empty;   
    public DateTime BirthDate { get; set; }
    public DateTime HireDate { get; set; }
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string ImagePath { get; set; } = string.Empty;
    public List<string> Roles { get; set; } = [];
    public List<string> Departments { get; set; } = [];
    public List<Guid> SubmissionsGuid { get; set; } = [];
}