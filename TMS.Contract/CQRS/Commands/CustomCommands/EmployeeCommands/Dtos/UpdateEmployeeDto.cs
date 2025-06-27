namespace TMS.Contract.CQRS.Commands.CustomCommands.EmployeeCommands.Dtos;

public class UpdateEmployeeDto : IDto
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? NationalIdentificationNumber { get; set; }
    public string ? ImagePath { get; set; }
    public DateTime? BirthDate { get; set; }
    public DateTime? HireDate { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
}