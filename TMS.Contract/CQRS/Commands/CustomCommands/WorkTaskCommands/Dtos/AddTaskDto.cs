namespace TMS.Contract.CQRS.Commands.CustomCommands.WorkTaskCommands.Dtos;

public class AddTaskDto : IDto
{
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

    public int AssignedToEmployeeId { get; set; }

    //public ICollection<int> ProjectIds { get; set; }
}


public class AddTaskResponseDto : IDto
{
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

    public int AssignedToEmployeeId { get; set; }

    //public ICollection<int> ProjectIds { get; set; }
}

