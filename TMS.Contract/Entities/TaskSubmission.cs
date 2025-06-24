using System;
using System.Collections.Generic;

namespace TMS.Contract.Entities;

public class TaskSubmission : Entity
{
    public Guid SubmissionUniqueIdentifier { get; init; } = Guid.NewGuid();
    public string Description { get; set; }
    public DateTime SubmissionDate { get; set; } = DateTime.UtcNow;
    public string Status { get; set; } = "Pending"; // Pending, Approved, Rejected
    public string? FeedbackComments { get; set; }

    // Relation to the WorkTask
    public int WorkTaskId { get; set; }
    public WorkTask WorkTask { get; set; }

    // Relation to the Employee who submitted
    public int SubmittedByEmployeeId { get; set; }
    public Employee SubmittedBy { get; set; }

    // Files attached to the submission
    public ICollection<SubmissionFile> Files { get; set; } = new List<SubmissionFile>();
}
