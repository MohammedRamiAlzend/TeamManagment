
using System.Linq.Expressions;
using TMS.Core.Entities.Interfaces;

namespace TMS.Server.Controllers.ControllersHelper;

public static class IncludeHelper
{
    public static Func<IQueryable<T>, IIncludableQueryable<T, object>>? GetIncludes<T>(
        string[]? includeProperties,
        Dictionary<string, Expression<Func<T, object>>> includeExpressions) where T : Entity
    {
        if (includeProperties is null || includeProperties.Length == 0) return null;
    
        var validIncludes = includeProperties.Where(includeExpressions.ContainsKey).ToList();
        if (validIncludes.Count == 0) return null;
    
        return query => (IIncludableQueryable<T, object>)BuildIncludeQuery(query, validIncludes, includeExpressions);
    }

    private static IQueryable<T> BuildIncludeQuery<T>(
        IQueryable<T> query, 
        IEnumerable<string> includeProperties,
        Dictionary<string, Expression<Func<T, object>>> includeExpressions) where T : Entity
    {
        foreach (var property in includeProperties)
        {
            var includeParts = property.Split('.');
        
            if (includeParts.Length == 1)
            {
                if (includeExpressions.TryGetValue(includeParts[0], out var expression))
                {
                    query = query.Include(expression);
                }
            }
            else
            {
                // For nested includes, fall back to string-based include
                // because ThenInclude with dynamic expressions is extremely complex
                query = query.Include(property);
            }
        }

        return query;
    }

}