namespace TMS.Contract.CQRS.Commands.CustomCommands.DepartmentCommands.Dtos;

public class CreatedDepartmentResponseDto 
{
    public string Name { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    
    public int? ParentDepartmentId { get; set; }

    public int TeamLeaderId { get; set; }
    public string TeamLeaderName { get; set; }

    public ICollection<string>? SubDepartmentNames { get; init; } = [];
    public ICollection<string> EmployeeNames { get; set; } = [];
}