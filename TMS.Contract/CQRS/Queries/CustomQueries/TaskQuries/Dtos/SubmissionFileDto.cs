using System;
using TMS.Contract.Entities;

namespace TMS.Contract.CQRS.Queries.CustomQueries.TaskQuries.Dtos;

public class SubmissionFileDto : IDto
{
    public required Guid FileUniqueIdentifier { get; init; } 
    public required string FileName { get; set; }
    public required string FilePath { get; set; }
    public required string FileExtension { get; set; }
    public required string OriginalFileName { get; set; }
    public required string ContentType { get; set; }
    public required long FileSize { get; set; }
    public required DateTime UploadedDate { get; set; }

}
