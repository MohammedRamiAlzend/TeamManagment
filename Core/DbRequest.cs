namespace TMS.Core;
public class DbRequest
{
    public bool IsSuccess { get; set; }
    public string? Message { get; set; }
}
public class DbRequest<T> : DbRequest where T : class
{
    public T? Data { get; set; }
}
