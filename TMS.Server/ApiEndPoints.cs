namespace TMS.Server;

public static class ApiEndPoints
{
    public const string ApiBase = "api";

    public static class EmployeesEndPoint
    {
        public const string Get = "{employeeId:int}";
        public const string GetAll = "";
        public const string GetAllPaginated = "paginated";
        public const string Update = "{employeeId:int}";
        public const string Delete = "{employeeId:int}";
    }
    public static class DepartmentsEndPoint
    {
        public const string Get = "{departmentId:int}";
        public const string GetAll = "";
        public const string GetAllPaginated = "paginated";
        public const string Includes = "DepartmentIncludes";
        public const string Create = "create-department";
        public const string Update = "{departmentId:int}";
        public const string Delete = "{departmentId:int}";
        public const string UpdateDepartmentTeamLeader = "update-teamleader";
    }
    public static class PermissionsEndPoint
    {
        public const string Get = "{permissonId:int}";
        public const string GetAll = "";
        public const string GetAllPaginated = "paginated";
        public const string Create = "";
    }
    public static class RolesEndPoint   
    {
        public const string Get = "{roleId:int}";
        public const string GetAll = "";
        public const string GetAllPaginated = "paginated";
    }
    
    public static class ProjectsEndPoint
    {
        public const string Get = "{projectId:int}";
        public const string GetAll = "";
        public const string GetAllPaginated = "paginated";
        public const string Create = "";
        public const string AddTasks = "add-task";
        public const string Update = "{projectId:int}";
        public const string Delete = "{projectId:int}";
    }
    public static class TasksEndPoint
    {
        public const string Get = "{taskGuidId:Guid}";
        public const string GetAll = "";
        public const string GetAllPaginated = "paginated";
        public const string Create = "";
        public const string Update = "{taskGuidId:Guid}";
        public const string Delete = "{taskGuidId:Guid}";
        public const string SubmitTask = "submit-task/{taskGuidId:Guid}";
        public const string GetSubmissionFiles = "get-submission-files/{taskGuidId:Guid}/files";
        public const string DownloadSubmissionFile = "download-submission-file/{taskGuidId:Guid}/files/{fileGuidId:int}/download";
        public const string DownloadAllFiles = "download-all-files/{taskGuidId:Guid}/download";
    }
    public static class TaskSubmissionsEndPoint
    {
        public const string GetAll = "task/{taskGuidId:Guid}/submissions";
        public const string GetById = "task/{taskGuidId:Guid}/submissions/{submissionGuidId:int}";
        public const string Create = "task/{taskGuidId:Guid}/submissions";
        public const string Update = "task/{taskGuidId:Guid}/submissions/{submissionGuidId:int}";
        public const string Delete = "task/{taskGuidId:Guid}/submissions/{submissionGuidId:int}";
    }
}