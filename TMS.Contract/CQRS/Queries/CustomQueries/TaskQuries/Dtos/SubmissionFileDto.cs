using System;
using TMS.Contract.Entities;

namespace TMS.Contract.CQRS.Queries.CustomQueries.TaskQuries.Dtos;

public class SubmissionFileDto : IDto
{
    public int Id { get; set; }
    public string FileName { get; set; }
    public string OriginalFileName { get; set; }
    public string ContentType { get; set; }
    public long FileSize { get; set; }
    public DateTime UploadedDate { get; set; }
}
