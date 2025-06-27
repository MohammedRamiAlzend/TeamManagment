namespace TMS.Application.Services.Interfaces.ProjectInterfaces;

public interface IProjectService
{
    Task<List<Employee>> GetEnrolledMembers(List<int> enrolledMembersIds);
    Task<List<WorkTask>> GetTasksForProject(List<Guid> tasksIds);
} 