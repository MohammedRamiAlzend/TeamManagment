using System.Net;
namespace TMS.Core;
public class ApiResponse
{
    public bool IsSuccess { get; set; }
    public string? Message { get; set; }
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
public class ApiResponse<TData> : ApiResponse
{
    public TData? Data { get; set; }
    public static ApiResponse<TData> Success(TData data, HttpStatusCode code = HttpStatusCode.OK, params string[] messages)
    {
        return new ApiResponse<TData>()
        {
            IsSuccess = true,
            Message = string.Join(", ", messages),
            Data = data,
            Code = code
        };
    }
    public static new ApiResponse<TData> Failure(HttpStatusCode code = HttpStatusCode.BadRequest, params string[] messages)
    {
        return new()
        {
            IsSuccess = false,
            Message = string.Join(", ", messages),
            Data = default,
            Code = code
        };
    }
}

public class PaginatedApiResponse<T> :ApiResponse<List<T>>
{
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }

    public static PaginatedApiResponse<T> Success(
        List<T> items,
        int totalCount,
        int pageNumber,
        int pageSize,
        HttpStatusCode code = HttpStatusCode.OK,
        params string[] messages)
    {
        return new PaginatedApiResponse<T>()
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
    public static new PaginatedApiResponse<T> Failure(HttpStatusCode code = HttpStatusCode.BadRequest, params string[] messages)
    {
        return new PaginatedApiResponse<T>()
        {
            IsSuccess = false,
            Message = string.Join(", ", messages),
            Data = [],
            TotalCount = 0,
            PageNumber = 0,
            PageSize = 0,
            Code = code
        };
    }
}
