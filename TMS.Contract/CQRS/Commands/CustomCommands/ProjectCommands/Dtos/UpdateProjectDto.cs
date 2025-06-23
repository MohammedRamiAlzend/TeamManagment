using TMS.Contract.Entities.Enums;

namespace TMS.Contract.CQRS.Commands.CustomCommands.ProjectCommands.Dtos;
public class UpdateProjectDto:IDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public ProjectStatus ProjectStatus { get; set; }
    public int DepartmentId { get; set; }
}