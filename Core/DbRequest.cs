namespace TMS.Core;

public class DbRequest
{
    public bool IsSuccess { get; set; }
    public string? Message { get; set; }

    public static DbRequest Success(params string[] messages)
    {
        return new DbRequest
        {
            IsSuccess = true,
            Message = string.Join(", ", messages)
        };
    }

    public static DbRequest Failure(params string[] messages)
    {
        return new DbRequest
        {
            IsSuccess = false,
            Message = string.Join(", ", messages)
        };
    }
}

public class DbRequest<T>
{
    public bool IsSuccess { get; set; }
    public string? Message { get; set; }
    public T? Data { get; set; }

    public static DbRequest<T> Success(params string[] messages)
    {
        return new DbRequest<T>
        {
            IsSuccess = true,
            Message = string.Join(", ", messages)
        };
    }

    public static DbRequest<T> Success(T data, params string[] messages)
    {
        return new DbRequest<T>
        {
            Data = data,
            IsSuccess = true,
            Message = string.Join(", ", messages)
        };
    }

    public static DbRequest<T> Failure(params string[] messages)
    {
        return new DbRequest<T>
        {
            IsSuccess = false,
            Message = string.Join(", ", messages)
        };
    }
}

public class PaginatedDbRequest<T> : DbRequest<T>
{
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public List<T> Items { get; set; } = [];

    public static PaginatedDbRequest<T> Success(
        List<T> items,
        int totalCount,
        int pageNumber,
        int pageSize,
        params string[] messages)
    {
        return new PaginatedDbRequest<T>
        {
            IsSuccess = true,
            Message = string.Join(", ", messages),
            Items = items,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }

    public new static PaginatedDbRequest<T> Failure(params string[] messages)
    {
        return new PaginatedDbRequest<T>
        {
            IsSuccess = false,
            Message = string.Join(", ", messages),
            TotalCount = 0,
            PageNumber = 0,
            PageSize = 0
        };
    }
}