namespace TMS.Core.Entities;

public class WorkTask : Entity
{
    //Should be unique
    //TODO:make an interceptor and check if the type then generate it and
    //save it in database
    public string TaskUniqueIdentifier { get; init; }
    public string Title { get; init; }
    public string? Description { get; init; }
    public string Status { get; init; }
    public string? Priority { get; init; }
    public DateTime? StartDate { get; init; }
    public DateTime? EndDate { get; init; }
    public DateTime? DeadLine { get; init; }
    public bool Accepted { get; init; }
    public int PointId { get; init; }
    public Point? Point { get; init; }
    public int? ProjectId { get; init; } 
    public Project? Project { get; init; }
    
}
