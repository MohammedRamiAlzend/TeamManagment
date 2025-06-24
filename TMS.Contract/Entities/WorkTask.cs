using System.Collections.Generic;

namespace TMS.Contract.Entities;

public class WorkTask : Entity
{
    public Guid TaskUniqueIdentifier { get; init; } = Guid.NewGuid();
    public string Title { get; set; }
    public string Description { get; set; }
    public string Status { get; set; }
    public string Priority { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public DateTime DeadLine { get; set; }
    public bool Accepted { get; set; }
    public int PointsValue { get; set; }

    // Submission related properties
    public bool AllowMultipleSubmissions { get; set; } = false;
    public bool RequiresSubmission { get; set; } = true;
    public DateTime? SubmissionDeadline { get; set; }
    public string SubmissionInstructions { get; set; }

    public int CreatedByEmployeeId { get; set; }
    public Employee CreatedBy { get; set; }

    public int AssignedToEmployeeId { get; set; }
    public Employee AssignedTo { get; set; }

    public ICollection<Project> Projects { get; set; }

    // Submissions related to this task
    public ICollection<TaskSubmission> Submissions { get; set; } = new List<TaskSubmission>();
}