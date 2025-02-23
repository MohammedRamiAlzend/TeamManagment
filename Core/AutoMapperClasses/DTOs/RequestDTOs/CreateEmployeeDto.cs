using System.ComponentModel.DataAnnotations;

namespace TMS.Core.AutoMapperClasses.DTOs.RequestDTOs;

public class CreateEmployeeDto : IDTO
{
    [Required]
    public required string FirstName { get; set; }

    [Required]
    public required string LastName { get; set; }

    public string? FatherName { get; set; }
    public string? MiddleName { get; set; }
    public string? MotherName { get; set; }

    [Required]
    public required string NationalIdentificationNumber { get; set; }

    public DateTime BirthDate { get; set; }
    public DateTime HireDate { get; set; }

    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }

    public ICollection<int>? DepartmentIds { get; set; }
}
