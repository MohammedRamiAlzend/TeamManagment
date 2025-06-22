namespace TMS.Contract.Responses;

public class GetPermissionResponse : IDto
{
    public string Name { get; set; }
    public ICollection<string> Roles { get; set; }
}