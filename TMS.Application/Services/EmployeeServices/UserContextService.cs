using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TMS.Application.Services.Interfaces.EmployeeInterfaces;
using TMS.Contract.Entities;
using TMS.Core.Interfaces;

namespace TMS.Application.Services.EmployeeServices
{
    public class UserContextService : IUserContextService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IEntityCommiter _commiter;

        public UserContextService(IHttpContextAccessor httpContextAccessor, IEntityCommiter commiter)
        {
            _httpContextAccessor = httpContextAccessor;
            _commiter = commiter;
        }

        public Guid? GetCurrentUserId()
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (Guid.TryParse(userIdClaim, out var userId))
            {
                return userId;
            }
            return null;
        }

        public async Task<Employee?> GetCurrentEmployeeAsync()
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                return null;
            var result = await _commiter.Employees.GetAsync(x => x.User.Id == userId);
            return result.Data;
        }
    }
} 