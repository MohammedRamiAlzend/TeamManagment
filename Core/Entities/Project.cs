using TMS.Core.Entities.Enums;

namespace TMS.Core.Entities;

public class Project :Entity
{
    public string Name { get; init; }
    public string? Description { get; init; }
    public DateTime StartDate { get; init; }
    public DateTime? EndDate { get; init; }
    public ProjectStatus Status { get; init; } 
    public int? ProjectManagerId { get; init; } 
    public Employee ProjectManager { get; init; }
    public ICollection<WorkTask> Tasks { get; init; }
    public ICollection<ProjectTeamMember> TeamMembers { get; init; }
}