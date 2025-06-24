using System;
using System.Collections.Generic;

namespace TMS.Contract.Entities;

public class TaskSubmission : Entity
{
    public Guid SubmissionUniqueIdentifier { get; init; } = Guid.NewGuid();

    public int WorkTaskId { get; set; }
    public WorkTask WorkTask { get; set; }

    public int SubmittedByEmployeeId { get; set; }
    public Employee SubmittedBy { get; set; }

    public DateTime SubmissionDate { get; set; }
    public string Description { get; set; }
    public string Status { get; set; } // Pending, Approved, Rejected

    public DateTime? ReviewedDate { get; set; }
    public string FeedbackComments { get; set; }

    public ICollection<SubmissionFile> Files { get; set; } = new List<SubmissionFile>();
}