namespace TMS.Application.Services;

public interface IUserContextService
{
    Guid? GetCurrentUserId();
    Task<Employee?> GetCurrentEmployeeAsync();
} 