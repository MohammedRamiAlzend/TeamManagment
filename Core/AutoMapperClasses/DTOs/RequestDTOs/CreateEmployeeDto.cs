using System.ComponentModel.DataAnnotations;

namespace TMS.Core.AutoMapperClasses.DTOs.RequestDTOs;

public class CreateEmployeeDto : IDto
{
    [Required]
    public required string FirstName { get; set; }
    [Required]
    public required string LastName { get; set; }
    [Required]
    public required string NationalIdentificationNumber { get; set; }
    [Required]
    public DateTime BirthDate { get; set; }
    [Required]
    public DateTime HireDate { get; set; }
    [Required]
    public required string Email { get; set; }
    [Required]
    public required string Phone { get; set; }
    [Required]
    public required string Address { get; set; }

    public ICollection<int>? DepartmentIds { get; set; }
}
