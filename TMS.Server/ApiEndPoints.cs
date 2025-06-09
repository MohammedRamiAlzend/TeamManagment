namespace TMS.Server;

public static class ApiEndPoints
{
    public const string ApiBase = "api";

    public static class Employees
    {
        // private const string Base = $"{ApiBase}/employees";
        
        public const string Get = $"{{employeeId:int}}";
        public const string GetAll = "";
        public const string GetAllPaginated = $"paginated";
        
        public  const string Create = "";
        public const string Update = $"{{employeeId:int}}";
        public const string Delete = $"{{employeeId:int}}";
    }
}