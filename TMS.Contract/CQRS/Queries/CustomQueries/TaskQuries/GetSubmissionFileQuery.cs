using MediatR;
using System;
using TMS.Contract.CommunicationModels;

namespace TMS.Contract.CQRS.Queries.CustomQueries.TaskQuries;

public record GetSubmissionFileQuery(Guid TaskId, Guid SubmissionId, int FileId) : IRequest<ApiResponse<SubmissionFileResult>>;

public class SubmissionFileResult
{
    public byte[] FileContents { get; set; }
    public string FileName { get; set; }
    public string ContentType { get; set; }
}
