namespace TMS.Core.Entities;

public class WorkTask : Entity
{
    //Should be unique
    //TODO:make an interceptor and check if the type then generate it and
    //save it in database
    public string TaskUniqueIdentifier { get; set; }
    public string Title { get; set; }
    public string? Description { get; set; }
    public string Status { get; set; }
    public string? Priority { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    [NotMapped]
    public DateTime? DeadLine { get; set; }
    public int PointId { get; set; }
    public Point Point { get; set; }
}
