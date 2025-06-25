using TMS.Contract.CQRS.Commands.CustomCommands.DepartmentCommands;
using TMS.Contract.CQRS.Commands.CustomCommands.DepartmentCommands.Dtos;
using TMS.Contract.CQRS.Queries.CustomQueries.DepartmentQuries;

namespace TMS.Core.AutoMapperClasses.ProfileClasses;

public class DepartmentProfile : Profile
{
    public DepartmentProfile()
    {
        CreateMap<Department, GetDepartmentResponse>()
            .ForMember(dest => dest.TeamLeaderName,
                opt => opt.MapFrom(scr => $"{scr.TeamLeader.FirstName} {scr.TeamLeader.LastName}"))
            .ForMember(dest => dest.EmployeesNamesAsDictionary,
                opt => opt.MapFrom(scr => MapEmployeeNames(scr.Employees)));

        CreateMap<Department, CreateDepartmentDto>();

        CreateMap<UpdateDepartmentCommand, Department>()
            .ForMember(dest => dest.Employees, opt => opt.Ignore())
            .ForMember(dest => dest.SubDepartments, opt => opt.Ignore())
            .ForMember(dest => dest.TeamLeader, opt => opt.Ignore())
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
    }

    private static Dictionary<int,string> MapEmployeeNames(ICollection<Employee> employees)
    {
        var dict = new Dictionary<int, string>();
        foreach (var employee in employees) {
            dict[employee.Id] = $"{employee.FirstName} {employee.LastName}";
        }
        return dict;
    }

}