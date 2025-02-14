namespace Core.Data.Entities;
public class TaskAssignment :IHasId,IAuditable,ISoftDeletable
{
    public int Id { get; set; }
    public DateTime AssignedAt { get; set; }

    public int TaskID { get; set; }
    [ForeignKey("TaskID")]
    public virtual WorkTask Task { get; set; }
    public int AssignedToEmployeeID { get; set; }
    [ForeignKey("AssignedToEmployeeID")]
    public virtual Employee AssignedToEmployee { get; set; }
    public int AssignedByEmployeeID { get; set; }
    [ForeignKey("AssignedByEmployeeID")]
    public virtual Employee AssignedByEmployee { get; set; }

    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

}
