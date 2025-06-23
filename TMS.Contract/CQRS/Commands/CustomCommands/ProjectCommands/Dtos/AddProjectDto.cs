namespace TMS.Contract.CQRS.Commands.CustomCommands.ProjectCommands.Dtos;

public class AddProjectDto : IDto
{
    [Required]
    public string ProjectName { get; set; }
    [Required]
    public string Description { get; set; }
    [Required]
    public DateTime StartDate { get; set; }
    [Required]
    public DateTime EndDate { get; set; }
    [Required]
    public int DepartmentId { get; set; }
    public List<int> EnrolledMembersIds { get; set; }
}