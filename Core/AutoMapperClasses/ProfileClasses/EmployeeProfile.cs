using AutoMapper;
using TMS.Core.AutoMapperClasses.DTOs.RequestDTOs;
using TMS.Core.AutoMapperClasses.DTOs.RequestDTOs.RequestEmployeeDTOS;
using TMS.Core.AutoMapperClasses.DTOs.ResponseDTOs;
using TMS.Core.AutoMapperClasses.DTOs.ResponseDTOs.ResponseDepartmentDTOS;
using TMS.Core.AutoMapperClasses.DTOs.ResponseDTOs.ResponseEmployeeDTOS;
using TMS.Core.Entities;

namespace TMS.Core.AutoMapperClasses.ProfileClasses;

public class EmployeeProfile : Profile
{
    public EmployeeProfile()
    {
        // CreateMap<CreateEmployeeDto, Employee>()
        //     .ForMember(dest => dest.CreatedTasks, opt => opt.Ignore())
        //     .ForMember(dest => dest.AssignedTasks, opt => opt.Ignore());

        // CreateMap<Department, DepartmentDto>();
    
        CreateMap<Employee, EmployeeDto>()
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"))
            .ForMember(dest => dest.Departments, opt => opt.MapFrom(src => src.Departments)); 

        CreateMap<UpdateEmployeeDto, Employee>()
            .ForMember(dest => dest.Departments, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedTasks, opt => opt.Ignore())
            .ForMember(dest => dest.AssignedTasks, opt => opt.Ignore())
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
    }

}
