using Microsoft.AspNetCore.Http;

namespace TMS.Contract.CQRS.Commands.CustomCommands.WorkTaskCommands.Dtos;

public class UpdateTaskSubmissionDto
{

    public Guid? WorkTaskId { get; set; }

    public int? SubmittedByEmployeeId { get; set; }

    public DateTime? SubmissionDate { get; set; }
    public string? Description { get; set; }
    public string? Status { get; set; } // Pending, Approved, Rejected

    public DateTime? ReviewedDate { get; set; }
    public string? FeedbackComments { get; set; }

    public ICollection<IFormFile>? Files { get; set; } = [];
}
