namespace TMS.Contract.CQRS.Commands.CustomCommands.ProjectCommands.Dtos;

public class AddTasksToProjectDto : IDto
{
    public int ProjectId { get; set; }
    public List<Guid> TasksIds { get; set; }
}