namespace TMS.Core.AutoMapperClasses.ProfileClasses;

public class DepartmentProfile : Profile
{
    public DepartmentProfile()
    {
        CreateMap<Department, GetDepartmentResponse>()
            .ForMember(dest => dest.TeamLeaderName,
                opt => opt.MapFrom(scr => $"{scr.TeamLeader.FirstName} {scr.TeamLeader.LastName}"))
            .ForMember(dest => dest.EmployeesNames,
                opt => opt.MapFrom(scr => MapEmployeeNames(scr.Employees)));

        CreateMap<Department, CreateDepartmentDto>();

        CreateMap<UpdateDepartmentDto, Department>()
            .ForMember(dest => dest.Employees, opt => opt.Ignore())
            .ForMember(dest => dest.SubDepartments, opt => opt.Ignore())
            .ForMember(dest => dest.TeamLeader, opt => opt.Ignore())
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
    }

    private static List<string> MapEmployeeNames(ICollection<Employee> employees)
    {
        return employees?.Select(employee => $"{employee.FirstName} {employee.LastName}").ToList() ?? [];
    }

}