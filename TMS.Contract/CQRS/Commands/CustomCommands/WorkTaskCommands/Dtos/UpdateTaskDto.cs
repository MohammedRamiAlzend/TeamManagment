namespace TMS.Contract.CQRS.Commands.CustomCommands.WorkTaskCommands.Dtos;

public class UpdateTaskDto : IDto
{
    public string? Title { get; set; } = null;
    public string? Description { get; set; } = null;
    public string? Status { get; set; } = null;
    public string? Priority { get; set; } = null;
    public DateTime? StartDate { get; set; } = null;
    public DateTime? EndDate { get; set; }  = null;
    public DateTime? DeadLine { get; set; } = null;
    public bool? Accepted { get; set; } = null;
    public int? PointsValue { get; set; } = null;
    public int? CreatedByEmployeeId { get; set; } = null;
    public int? AssignedToEmployeeId { get; set; } = null;
    public ICollection<int>? ProjectIds { get; set; } = null;
}