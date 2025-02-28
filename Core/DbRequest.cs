using System.Net;
namespace TMS.Core;
public class DbRequest
{
    public bool IsSuccess { get; set; }
    public string? Message { get; set; }
    public object? Data { get; set; }
    public static DbRequest Success(params string[] messages)
    {
        return new DbRequest()
        {
            IsSuccess = true,
            Message = string.Join(", ", messages),
        };
    }
    public static DbRequest Success<T>(T Data, params string[] messages)
    {
        return new DbRequest()
        {
            Data = Data,
            IsSuccess = true,
            Message = string.Join(", ", messages),
        };
    }

    public static DbRequest Failure(params string[] messages)
    {
        return new()
        {
            IsSuccess = false,
            Message = string.Join(", ", messages),
        };
    }

}

public class PaginatedDbRequest : DbRequest
{
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public static PaginatedDbRequest Success<T>(
        List<T> items,
        int totalCount,
        int pageNumber,
        int pageSize,
        params string[] messages)
    {
        return new PaginatedDbRequest()
        {
            IsSuccess = true,
            Message = string.Join(", ", messages),
            Data = items,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize,
        };
    }
    public static new PaginatedDbRequest Failure(params string[] messages)
    {
        return new PaginatedDbRequest()
        {
            IsSuccess = false,
            Message = string.Join(", ", messages),
            TotalCount = 0,
            PageNumber = 0,
            PageSize = 0,
        };
    }
}
