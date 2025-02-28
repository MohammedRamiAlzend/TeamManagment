namespace TMS.Core.AutoMapperClasses.DTOs.RequestDTOs;

public class UpdateEmployeeDto : IDTO
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? FatherName { get; set; }
    public string? MiddleName { get; set; }
    public string? MotherName { get; set; }
    public string? NationalIdentificationNumber { get; set; }
    public DateTime? BirthDate { get; set; }
    public DateTime? HireDate { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
}
