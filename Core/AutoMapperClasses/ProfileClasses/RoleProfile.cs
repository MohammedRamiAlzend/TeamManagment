using TMS.Contract.CQRS.Queries.CustomQueries.RoleQuries;

namespace TMS.Core.AutoMapperClasses.ProfileClasses;

public class RoleProfile : Profile
{
    public RoleProfile()
    {
        CreateMap<Role, GetRoleResponse>()
            .ForMember(x=>x.Permissions,
                dest=>dest.MapFrom(scr => MapPermissionsNames(scr.Permissions)));
    }

    private static List<string> MapPermissionsNames(ICollection<Permission> roles)
    {
        return roles?.Select(permission => permission.Name).ToList() ?? [];
    }
}