namespace TMS.Contract.CQRS.Queries.CustomQueries.RoleQuries;

public class GetRoleResponse : IDto
{
    public string Name { get; set; }
    public ICollection<string> Permissions { get; set; }
}