using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;

namespace TMS.Infrastructure.Services;
public class PermissionPolicyProvider : IAuthorizationPolicyProvider
{
    private readonly DefaultAuthorizationPolicyProvider _fallbackProvider;
    private readonly IMemoryCache _cache;

    public PermissionPolicyProvider(IOptions<AuthorizationOptions> options, IMemoryCache cache)
    {
        _fallbackProvider = new DefaultAuthorizationPolicyProvider(options);
        _cache = cache;
    }

    public Task<AuthorizationPolicy> GetDefaultPolicyAsync() => _fallbackProvider.GetDefaultPolicyAsync();
    public Task<AuthorizationPolicy?> GetFallbackPolicyAsync() => _fallbackProvider.GetFallbackPolicyAsync();

    public Task<AuthorizationPolicy> GetPolicyAsync(string policyName)
    {
        if (!policyName.StartsWith(HasPermissionAttribute.PolicyPrefix))
        {
            return _fallbackProvider.GetPolicyAsync(policyName);
        }

        return _cache.GetOrCreateAsync(policyName, entry =>
        {
            entry.SetSlidingExpiration(TimeSpan.FromHours(1));

            var policyString = policyName.Substring(HasPermissionAttribute.PolicyPrefix.Length);
            var parts = policyString.Split(':', 2);

            if (parts.Length == 2)
            {
                var op = Enum.Parse<LogicalOperator>(parts[0]);
                var permissions = parts[1].Split(HasPermissionAttribute.Separator);
                
                var policy = new AuthorizationPolicyBuilder();
                policy.AddRequirements(new LogicalPermissionRequirement(op, permissions));
                
                return Task.FromResult(policy.Build());
            }
            return _fallbackProvider.GetPolicyAsync(policyName);
        });
    }
}
