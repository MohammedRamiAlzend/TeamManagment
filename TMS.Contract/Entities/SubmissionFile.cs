using System;

namespace TMS.Contract.Entities;

public class SubmissionFile : Entity
{
    public Guid FileUniqueIdentifier { get; init; } = Guid.NewGuid();
    public string FileName { get; set; }
    public string OriginalFileName { get; set; }
    public string FilePath { get; set; }
    public string FileExtension { get; set; }
    public long FileSize { get; set; } // in bytes
    public string ContentType { get; set; }

    // Relation to the TaskSubmission
    public int TaskSubmissionId { get; set; }
    public TaskSubmission TaskSubmission { get; set; }
}
