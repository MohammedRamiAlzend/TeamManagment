using AutoMapper;
using TMS.Core.AutoMapperClasses.DTOs;
using TMS.Core.Entities;

namespace TMS.Core.AutoMapperClasses.ProfileClasses;

public class EmployeeProfile : Profile
{
    public EmployeeProfile()
    {
        CreateMap<Employee, EmployeeDTO>().ReverseMap();
    }
}
