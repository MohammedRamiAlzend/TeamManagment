namespace TMS.Core.Entities;

public class ProjectTeamMember : Entity
{
    public int ProjectId { get; init; }
    public Project Project { get; init; }
    
    public int EmployeeId { get; init; }
    public Employee Employee { get; init; }
    
    public string RoleInProject { get; init; }
    public DateTime JoinedDate { get; init; }
    public DateTime? LeftDate { get; init; }

}