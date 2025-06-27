namespace TMS.Application.Services.Interfaces.EmployeeInterfaces;

public interface IUserContextService
{
    Guid? GetCurrentUserId();
    Task<Employee?> GetCurrentEmployeeAsync();
} 