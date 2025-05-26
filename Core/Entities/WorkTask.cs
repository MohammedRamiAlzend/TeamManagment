namespace TMS.Core.Entities;

public class WorkTask : Entity
{
    //Should be unique
    //TODO:make an interceptor and check if the type then generate it and
    //save it in database
    public Guid TaskUniqueIdentifier { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string Status { get; set; }
    public string Priority { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public DateTime DeadLine { get; set; }
    public bool Accepted { get; set; }
    public int PointsValue { get; set; }
    
    public int CreatedByEmployeeId { get; set; }
    public Employee CreatedBy { get; set; }
    
    public int AssignedToEmployeeId { get; set; }
    public Employee AssignedTo { get; set; }

    public ICollection<Project> Projects { get; set; }
    
}
