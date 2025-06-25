namespace TMS.Contract.CQRS.Commands.CustomCommands.DepartmentCommands.Dtos;

public class CreateDepartmentDto : IDto
{
    [Required]
    public string Name { get; set; }

    [Required]
    public string Email { get; set; }

    [Required]
    public string PhoneNumber { get; set; }


    [Required]
    public int TeamLeaderId { get; set; }
    public int? ParentDepartmentId { get; set; }
    
    public ICollection<int> EnrolledEmployeeIds { get; set; }
}