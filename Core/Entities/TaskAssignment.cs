namespace TMS.Core.Entities;

public class TaskAssignment : Entity
{
    public DateTime AssignedAt { get; init; }

    public int TaskID { get; init; }
    [ForeignKey("TaskID")]
    public virtual WorkTask Task { get; init; }
    public int AssignedToEmployeeID { get; init; }
    [ForeignKey("AssignedToEmployeeID")]
    public virtual Employee AssignedToEmployee { get; init; }
    public int AssignedByEmployeeID { get; init; }
    [ForeignKey("AssignedByEmployeeID")]
    public virtual Employee AssignedByEmployee { get; init; }
}
