namespace TMS.Application.Services;

public interface IProjectService
{
    Task<List<Employee>> GetEnrolledMembers(List<int> enrolledMembersIds);
    Task<List<WorkTask>> GetTasksForProject(List<Guid> tasksIds);
} 