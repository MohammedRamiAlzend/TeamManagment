namespace TMS.Contract.CommunicationModels;

public class ApiResponse
{
    public bool IsSuccess { get; set; }
    public string? Message { get; set; }
    public HttpStatusCode? Code { get; set; }

    public static ApiResponse Success(HttpStatusCode code = HttpStatusCode.OK, params string[] messages)
    {
        return new ApiResponse
        {
            IsSuccess = true,
            Message = string.Join(", ", messages),
            Code = code
        };
    }

    public static ApiResponse Failure(HttpStatusCode code = HttpStatusCode.BadRequest, params string[] messages)
    {
        return new ApiResponse
        {
            IsSuccess = false,
            Message = string.Join(", ", messages),
            Code = code
        };
    }


    public static implicit operator ApiResponse(DbRequest request)
    {
        return new ApiResponse
        {
            IsSuccess = request.IsSuccess,
            Message = request.Message,
            Code = request.IsSuccess ? HttpStatusCode.OK : HttpStatusCode.BadRequest
        };
    }
}

public class ApiResponse<T> : ApiResponse
{
    public T? Data { get; set; }

    public new static ApiResponse<T> Success(HttpStatusCode code = HttpStatusCode.OK, params string[] messages)
    {
        return new ApiResponse<T>
        {
            IsSuccess = true,
            Message = string.Join(", ", messages),
            Code = code
        };
    }

    public static ApiResponse<T> Success(T data, HttpStatusCode code = HttpStatusCode.OK, params string[] messages)
    {
        return new ApiResponse<T>
        {
            Data = data,
            IsSuccess = true,
            Message = string.Join(", ", messages),
            Code = code
        };
    }

    public new static ApiResponse<T> Failure(HttpStatusCode code = HttpStatusCode.BadRequest, params string[] messages)
    {
        return new ApiResponse<T>
        {
            IsSuccess = false,
            Message = string.Join(", ", messages),
            Code = code
        };
    }


    public static implicit operator ApiResponse<T>(DbRequest<T> request)
    {
        return new ApiResponse<T>
        {
            IsSuccess = request.IsSuccess,
            Message = request.Message,
            Data = request.Data,
            Code = request.IsSuccess ? HttpStatusCode.OK : HttpStatusCode.BadRequest
        };
    }
}

public class PaginatedApiResponse<T> : ApiResponse<T>
{
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public List<T> Items { get; set; } = [];

    public static PaginatedApiResponse<T> Success(
        List<T> items,
        int totalCount,
        int pageNumber,
        int pageSize,
        HttpStatusCode code = HttpStatusCode.OK,
        params string[] messages)
    {
        return new PaginatedApiResponse<T>
        {
            IsSuccess = true,
            Message = string.Join(", ", messages),
            Items = items,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize,
            Code = code
        };
    }

    public new static PaginatedApiResponse<T> Failure(HttpStatusCode code = HttpStatusCode.BadRequest,
        params string[] messages)
    {
        return new PaginatedApiResponse<T>
        {
            IsSuccess = false,
            Message = string.Join(", ", messages),
            TotalCount = 0,
            PageNumber = 0,
            PageSize = 0,
            Code = code
        };
    }

    public static implicit operator PaginatedApiResponse<T>(PaginatedDbRequest<T> request)
    {
        return new PaginatedApiResponse<T>
        {
            IsSuccess = request.IsSuccess,
            Message = request.Message,
            Data = request.Data,
            TotalCount = request.TotalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            Code = request.IsSuccess ? HttpStatusCode.OK : HttpStatusCode.BadRequest
        };
    }
}