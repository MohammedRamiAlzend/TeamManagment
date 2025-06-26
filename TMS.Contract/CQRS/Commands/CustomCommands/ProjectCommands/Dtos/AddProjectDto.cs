namespace TMS.Contract.CQRS.Commands.CustomCommands.ProjectCommands.Dtos;

public class AddProjectDto : IDto
{
    public int Id { get; set; }
    public string ProjectName { get; set; }
    public string Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int DepartmentId { get; set; }
    public List<int> EnrolledMembersIds { get; set; }
    
    public List<Guid> GuidTasks { get; set; }
    
}