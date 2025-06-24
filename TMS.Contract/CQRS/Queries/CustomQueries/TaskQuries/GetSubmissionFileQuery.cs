using System;
using TMS.Contract.CommunicationModels;

namespace TMS.Contract.CQRS.Queries.CustomQueries.TaskQuries;

public record GetSubmissionFileQuery(Guid TaskId, int FileId) : IRequest<ApiResponse<SubmissionFileResult>>;
public record GetAllSubmissionsFilesQuery(Guid TaskId) : IRequest<ApiResponse<ZipSubmissionFileResult>>;

public class SubmissionFileResult
{
    public byte[] FileContents { get; set; }
    public string FileName { get; set; }
    public string ContentType { get; set; }
}

public class ZipSubmissionFileResult
{
    public byte[] ZipFileContents { get; set; }
    public string ZipFileName { get; set; }
    public string ContentType { get; set; } = "application/zip";
}
