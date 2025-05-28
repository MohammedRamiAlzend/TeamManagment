using TMS.Core.AutoMapperClasses.DTOs.RequestDTOs.RequestDepartmentsDTOS;
using TMS.Core.AutoMapperClasses.DTOs.RequestDTOs.RequestEmployeeDTOS;
using TMS.Core.AutoMapperClasses.DTOs.ResponseDTOs.ResponseDepartmentDTOS;
using TMS.Core.AutoMapperClasses.DTOs.ResponseDTOs.ResponseEmployeeDTOS;

namespace TMS.Core.AutoMapperClasses.ProfileClasses;

public class DepartmentProfile : Profile
{
    private static List<string> MapEmployeeNames(ICollection<Employee> employees)
    {
        return employees?.Select(employee => $"{employee.FirstName} {employee.LastName}").ToList() ?? [];
    }

    public DepartmentProfile()
    {
        CreateMap<Department, DepartmentDto>()
            .ForMember(dest => dest.EmployeesNames,
                opt => opt.MapFrom(scr => MapEmployeeNames(scr.Employees)));

        // CreateMap<List<Department>, List<DepartmentDto>>();
           

        CreateMap<Department, CreateDepartmentDto>();
       
        CreateMap<UpdateDepartmentDto,Department>()
            .ForMember(dest => dest.Employees, opt => opt.Ignore())
            .ForMember(dest=>dest.SubDepartments,opt=>opt.Ignore())
            .ForMember(dest=>dest.TeamLeader , opt => opt.Ignore())
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

    }
}