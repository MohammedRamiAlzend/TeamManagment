using System.Net;
namespace TMS.Core;
public class ApiResponse
{
    public bool IsSuccess { get; set; }
    public string? Message { get; set; }
    public object? Data { get; set; }
    public HttpStatusCode? Code { get; set; }
    public static ApiResponse Success(HttpStatusCode code = HttpStatusCode.OK, params string[] messages)
    {
        return new ApiResponse()
        {
            IsSuccess = true,
            Message = string.Join(", ", messages),
            Code = code
        };
    }
    public static ApiResponse Success<T>(T Data, HttpStatusCode code = HttpStatusCode.OK, params string[] messages)
    {
        return new ApiResponse()
        {
            Data = Data,
            IsSuccess = true,
            Message = string.Join(", ", messages),
            Code = code
        };
    }

    public static ApiResponse Failure(HttpStatusCode code = HttpStatusCode.BadRequest, params string[] messages)
    {
        return new()
        {
            IsSuccess = false,
            Message = string.Join(", ", messages),
            Code = code
        };
    }

}

public class PaginatedApiResponse :ApiResponse
{
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public static PaginatedApiResponse Success<T>(
        List<T> items,
        int totalCount,
        int pageNumber,
        int pageSize,
        HttpStatusCode code = HttpStatusCode.OK,
        params string[] messages)
    {
        return new PaginatedApiResponse()
        {
            IsSuccess = true,
            Message = string.Join(", ", messages),
            Data = items,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize,
            Code = code
        };
    }
    public static new PaginatedApiResponse Failure(HttpStatusCode code = HttpStatusCode.BadRequest, params string[] messages)
    {
        return new PaginatedApiResponse()
        {
            IsSuccess = false,
            Message = string.Join(", ", messages),
            TotalCount = 0,
            PageNumber = 0,
            PageSize = 0,
            Code = code
        };
    }
}
