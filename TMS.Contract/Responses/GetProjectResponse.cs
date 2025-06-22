namespace TMS.Contract.Responses;

public class GetProjectResponse : IDto
{
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Status { get; set; }
    public string DepartmentName { get; set; }

    public List<string> TeamMembers { get; set; }
    public Dictionary<Guid,string> Tasks { get; set; }
}