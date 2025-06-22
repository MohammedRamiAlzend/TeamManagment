namespace TMS.Contract.Responses;

public class GetRoleResponse : IDto
{
    public string Name { get; set; }
    public ICollection<string> Permissions { get; set; }
}