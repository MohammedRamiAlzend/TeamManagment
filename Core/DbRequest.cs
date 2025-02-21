namespace TMS.Core;
/// <summary>
/// Represents a paginated result.
/// </summary>
public class PaginatedDbRequest<T>
{
    public List<T> Items { get; set; } = [];
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
}

/// <summary>
/// Represents a standardized response for database operations.
/// </summary>
public class DbRequest
{
    public bool IsSuccess { get; set; }
    public string? Message { get; set; }

    public static DbRequest Success(params string[] messages)
        => new()
        {
            IsSuccess = true,
            Message = string.Join(", ", messages)
        };
    public static DbRequest Failure(params string[] messages) => new()
    {
        IsSuccess = false,
        Message = string.Join(", ", messages)
    };


}

/// <summary>
/// Represents a standardized response for database operations with data.
/// </summary>
public class DbRequest<TData> : DbRequest
{
    public TData Data { get; set; }

    public static DbRequest<TData> Success(TData data, params string[] messages)
        => new()
        {
            IsSuccess = true,
            Message = string.Join(", ", messages),
            Data = data
        };

    public static new DbRequest<TData> Failure(params string[] messages) => new()
    {
        IsSuccess = false,
        Message = string.Join(", ", messages)
    };


}
