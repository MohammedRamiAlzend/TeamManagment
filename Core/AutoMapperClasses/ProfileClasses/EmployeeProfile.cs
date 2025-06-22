using TMS.Contract.CQRS.Commands.CustomCommands.EmployeeCommands.Dtos;
using TMS.Contract.CQRS.Queries.CustomQueries.EmployeeQuries;

namespace TMS.Core.AutoMapperClasses.ProfileClasses;
public class EmployeeProfile : Profile
{
    public EmployeeProfile()
    {
        CreateMap<UpdateEmployeeDto, Employee>()
            .ForMember(dest => dest.Departments, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedTasks, opt => opt.Ignore())
            .ForMember(dest => dest.AssignedTasks, opt => opt.Ignore())
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        
        CreateMap<Employee,GetEmployeeResponse>()
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"))
            .ForMember(dest => dest.Roles, opt => opt.MapFrom(scr => MapRoleNames(scr.User.Roles)))
            .ForMember(dest => dest.Departments, opt => opt.MapFrom(scr => MapDepartmentNames(scr.Departments)));
        
        
    }
    private static List<string> MapDepartmentNames(ICollection<Department> departments)
    {
        return departments?.Select(department => department.Name).ToList() ?? [];
    }
    private static List<string> MapRoleNames(ICollection<Role> roles)
    {
        return roles?.Select(role => role.Name).ToList() ?? [];
    }
}