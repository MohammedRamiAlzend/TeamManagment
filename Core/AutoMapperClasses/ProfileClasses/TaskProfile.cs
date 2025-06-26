using TMS.Contract.CQRS.Commands.CustomCommands.WorkTaskCommands.Dtos;
using TMS.Contract.CQRS.Queries.CustomQueries.TaskQuries;
using TMS.Contract.CQRS.Queries.CustomQueries.TaskQuries.Dtos;

namespace TMS.Core.AutoMapperClasses.ProfileClasses;

public class TaskProfile: Profile
{
    public TaskProfile()
    {
        CreateMap<WorkTask, GetTaskResponse>()
            .ForMember(dest => dest.CreatedByName, opt =>
                opt.MapFrom(scr => scr.CreatedBy.FirstName + " " + scr.CreatedBy.LastName))
            .ForMember(dest => dest.AssignedToName, opt =>
                opt.MapFrom(scr => scr.AssignedTo.FirstName + " " + scr.AssignedTo.LastName))
            .ForMember(dest => dest.ProjectIdNames, opt =>
                opt.MapFrom(scr => scr.Projects.ToDictionary(k => k.Id, v => v.Name)))
            .ForMember(dest => dest.Submissions, opt =>
                opt.MapFrom(src => src.Submissions.Select(sub => new TaskSubmissionSummaryDto
                {
                    SubmissionUniqueIdentifier = sub.SubmissionUniqueIdentifier,
                    FileUniqueIdentifiers = sub.Files.Select(f => f.FileUniqueIdentifier).ToList()
                }).ToList()));

        //CreateMap<WorkTask, AddTaskDto>()
        //    .ForMember(dest => dest.ProjectIds, opt =>
        //        opt.MapFrom(scr => scr.Projects.Select(x => x.Id).ToList()));

        //CreateMap<WorkTask, AddTaskResponseDto>()
        //    .ForMember(dest => dest.ProjectIds, opt =>
        //        opt.MapFrom(scr => scr.Projects.Select(x => x.Id).ToList()));


        CreateMap<WorkTask, UpdateTaskDto>()
            .ForMember(dest => dest.ProjectIds, opt =>
                opt.MapFrom(scr => scr.Projects.Select(x => x.Id).ToList()));

        CreateMap<AddTaskDto, WorkTask>()
            .ForMember(dest => dest.Projects, opt => opt.Ignore())
            .ReverseMap();

        CreateMap<AddTaskResponseDto, WorkTask>()
            .ForMember(dest => dest.Projects, opt => opt.Ignore())
            .ReverseMap();

        CreateMap<TaskSubmission, TaskSubmissionResponseDto>()
            .ForMember(dest => dest.WorkTaskGuid, opt => opt.MapFrom(src => src.WorkTask.TaskUniqueIdentifier))
            .ForMember(dest => dest.SubmittedByEmployeeId, opt => opt.MapFrom(src => src.SubmittedByEmployeeId))
            .ForMember(dest => dest.FileIds, opt => opt.MapFrom(src => src.Files.Select(f => f.FileUniqueIdentifier).ToList()));
    }
}