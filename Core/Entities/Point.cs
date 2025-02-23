namespace TMS.Core.Entities;

public class Point : Entity
{
    public int PointValue { get; set; }
    public string PointType { get; set; }
    public string? Reason { get; set; }
    public int? TaskID { get; set; }
    [ForeignKey("TaskID")]
    public virtual WorkTask? Task { get; set; }
    public int? AssignedToEmployeeID { get; set; }
    [ForeignKey("AssignedToEmployeeID")]
    public virtual Employee? AssignedToEmployee { get; set; }
    public int? AssignedByEmployeeID { get; set; }
    [ForeignKey("AssignedByEmployeeID")]
    public virtual Employee? AssignedByEmployee { get; set; }
}