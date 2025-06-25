namespace TMS.Contract.CQRS.Commands.CustomCommands.DepartmentCommands.Dtos;

public class UpdateDepartmentDto : IDto
{
    public string? Name { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public int? ParentDepartmentId { get; set; }
    public int? TeamLeaderId { get; set; }
    public ICollection<int>? EnrolledEmployeeIds { get; set; }
}