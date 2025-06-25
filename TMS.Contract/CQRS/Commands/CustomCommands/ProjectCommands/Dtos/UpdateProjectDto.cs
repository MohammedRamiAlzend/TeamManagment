using TMS.Contract.Entities.Enums;

namespace TMS.Contract.CQRS.Commands.CustomCommands.ProjectCommands.Dtos;
public class UpdateProjectDto:IDto
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public ProjectStatus? ProjectStatus { get; set; }
    public int? DepartmentId { get; set; }
    public List<int>? EnrolledMembersIds { get; set; }
    public ICollection<Guid>? GuidTasks { get; set; }
    
}