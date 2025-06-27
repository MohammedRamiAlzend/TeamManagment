using TMS.Contract.CQRS.Commands.CustomCommands.ProjectCommands.Dtos;
using TMS.Contract.CQRS.Queries.CustomQueries.ProjectQuries;

namespace TMS.Core.AutoMapperClasses.ProfileClasses;

public class ProjectProfile : Profile
{
    public ProjectProfile()
    {
        CreateMap<Project, GetProjectResponse>()
            .ForMember(dest=>dest.DepartmentName,
                opt=>opt.MapFrom(scr=>scr.Department.Name))
            .ForMember(dest=> dest.TeamMembers,opt=>
                opt.MapFrom(scr=>scr.TeamMembers.Select(x=>$"{x.FirstName} {x.LastName}")))
            .ForMember(dest => dest.Tasks,opt=> 
                opt.MapFrom(scr => scr.Tasks.ToDictionary(k=>k.TaskUniqueIdentifier,v=>v.Title)));
    }
}