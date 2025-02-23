using AutoMapper;
using TMS.Core.AutoMapperClasses.DTOs.RequestDTOs;
using TMS.Core.AutoMapperClasses.DTOs.ResponseDTOs;
using TMS.Core.Entities;

namespace TMS.Core.AutoMapperClasses.ProfileClasses;

public class EmployeeProfile : Profile
{
    public EmployeeProfile()
    {
        //CreateMap<Employee, CreateEmployeeDto>().ReverseMap();

        CreateMap<CreateEmployeeDto, Employee>()
           .ForMember(dest => dest.Departments, opt => opt.Ignore()); 

        CreateMap<Employee, EmployeeDto>()
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"))
            .ForMember(dest => dest.Departments, opt => opt.MapFrom(src => src.Departments.Select(d => d.Name).ToList()));

    }
}
