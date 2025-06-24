namespace TMS.Contract.CQRS.Commands.CustomCommands.ProjectCommands.Dtos;

public class AddProjectDto : IDto
{
    public string ProjectName { get; set; }
    public string Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int DepartmentId { get; set; }
    public List<int> EnrolledMembersIds { get; set; }
    
    public List<Guid> Tasks { get; set; }
    
}