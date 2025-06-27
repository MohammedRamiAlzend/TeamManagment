using System.Net;
using System.Threading;
using System.Threading.Tasks;
using TMS.Application.Services.Interfaces.TaskInterfaces;
using TMS.Contract.CommunicationModels;
using TMS.Contract.CQRS.Commands.CustomCommands.WorkTaskCommands.Dtos;
using TMS.Contract.Entities;
using TMS.Contract.Entities.Interfaces;
using TMS.Core.Interfaces;
using AutoMapper;
using TMS.Contract.CQRS.Queries.CustomQueries.TaskQuries.Dtos;

namespace TMS.Application.Handlers.CustomHandlers.WorkTaskHandlers.CommandHanlders
{
    public record UpdateTaskSubmissionCommand(Guid SubmissionGuid, UpdateTaskSubmissionDto Submission) : IRequest<ApiResponse>;

    public class UpdateTaskSubmissionCommandHandler : IRequestHandler<UpdateTaskSubmissionCommand, ApiResponse>
    {
        private readonly IEntityCommiter _commiter;
        private readonly ITaskSubmissionFileService _fileService;
        private readonly IMapper _mapper;
        public UpdateTaskSubmissionCommandHandler(IEntityCommiter commiter, ITaskSubmissionFileService fileService, IMapper mapper) { 
            _commiter = commiter;
            _fileService = fileService;
            _mapper = mapper;
        }

        public async Task<ApiResponse> Handle(UpdateTaskSubmissionCommand request, CancellationToken cancellationToken)
        {
            var getResult = await _commiter.TaskSubmissions.GetAsync(x => x.SubmissionUniqueIdentifier == request.SubmissionGuid,
                include: i=>   i.Include(x=>x.WorkTask)
                                .Include(x=>x.SubmittedBy)
                                .Include(x=>x.Files));


            if (!getResult.IsSuccess || getResult.Data == null)
                return ApiResponse.Failure(HttpStatusCode.NotFound, "Task submission not found.");

            if(request.Submission.Description is not null)
                getResult.Data.Description = request.Submission.Description;
            
            if(request.Submission.Status is not null)
                getResult.Data.Status = request.Submission.Status;
            
            if(request.Submission.FeedbackComments is not null)
                getResult.Data.FeedbackComments = request.Submission.FeedbackComments;
            
            if(request.Submission.ReviewedDate is not null)
                getResult.Data.ReviewedDate = request.Submission.ReviewedDate;
            
            if(request.Submission.WorkTaskId is not null)
            {
                var getTask = await _commiter.Tasks.GetAsync(x => x.TaskUniqueIdentifier == request.Submission.WorkTaskId);
                if (getTask.IsSuccess is false || getTask.Data is null) return ApiResponse.Failure(HttpStatusCode.NotFound,getTask.Message??$"Task with id {request.Submission.WorkTaskId} was not found");
                getResult.Data.WorkTaskId = getTask.Data.Id;
            }
            
            if(request.Submission.SubmittedByEmployeeId is not null)
                getResult.Data.SubmittedByEmployeeId = request.Submission.SubmittedByEmployeeId.Value;
            
            if(request.Submission.SubmissionDate is not null)
                getResult.Data.SubmissionDate = request.Submission.SubmissionDate.Value;

            // Handle file update: delete old files, add new ones if provided
            if (request.Submission.Files != null && request.Submission.Files.Any())
            {
                // Remove old files from DB and disk
                var oldFiles = getResult.Data.Files.ToList();
                foreach (var file in oldFiles)
                {
                    // Remove file from DB
                    await _commiter.SubmissionFiles.RemoveAsync(f => f.Id == file.Id);
                    // Remove file from disk
                    var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, file.FilePath);
                    if (System.IO.File.Exists(filePath))
                        System.IO.File.Delete(filePath);
                }
                await _commiter.CommitAsync(cancellationToken);

                // Save new files
                // If DTO uses IFormFile, cast/convert as needed
                if (request.Submission.Files.First() is Microsoft.AspNetCore.Http.IFormFile)
                {
                    var formFiles = request.Submission.Files;
                    await _fileService.SaveSubmissionFiles(getResult.Data, formFiles, cancellationToken);
                }
                // else: handle other file types as needed
            }

            var updateResult = await _commiter.TaskSubmissions.UpdateAsync(getResult.Data);
            await _commiter.CommitAsync(cancellationToken);
            if (!updateResult.IsSuccess )
                return ApiResponse.Failure(HttpStatusCode.NotFound, updateResult.Message ?? "Submission not found.");
            return ApiResponse.Success(HttpStatusCode.OK, "Task submission updated.");
        }
    }
} 