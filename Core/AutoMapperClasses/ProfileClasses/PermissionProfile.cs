using TMS.Contract.CQRS.Queries.CustomQueries.PermissionQuries;

namespace TMS.Core.AutoMapperClasses.ProfileClasses;

public class PermissionProfile : Profile
{
    public PermissionProfile()
    {
        CreateMap<Permission, GetPermissionResponse>()
            .ForMember(dest=>dest.Roles,
                opt=> opt.MapFrom(scr => MapRoleNames(scr.Roles)));
    }
    
    private static List<string> MapRoleNames(ICollection<Role> roles)
    {
        return roles?.Select(role => role.Name).ToList() ?? [];
    }
}