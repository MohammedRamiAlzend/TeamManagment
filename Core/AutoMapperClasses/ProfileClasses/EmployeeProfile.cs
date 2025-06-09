namespace TMS.Core.AutoMapperClasses.ProfileClasses;
public class EmployeeProfile : Profile
{
    public EmployeeProfile()
    {
        CreateMap<Employee, EmployeeDto>()
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"))
            .ForMember(dest => dest.Departments, opt => opt.MapFrom(src => src.Departments));

        CreateMap<UpdateEmployeeDto, Employee>()
            .ForMember(dest => dest.Departments, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedTasks, opt => opt.Ignore())
            .ForMember(dest => dest.AssignedTasks, opt => opt.Ignore())
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        
        CreateMap<Employee,GetEmployeeResponse>()
            .ForMember(dest => dest.Roles, opt => opt.MapFrom(scr => scr.User.Roles.Select(s=>s.Name)))
            .ForMember(dest => dest.Departments, opt => opt.MapFrom(scr => scr.Departments.Select(s=>s.Name)));
        
        
    }
}