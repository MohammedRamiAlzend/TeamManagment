namespace TMS.Application.Handlers.CustomHandlers.DepartmentHandlers;

public class DeleteDepartmentCommandHandler(IEntityCommiter commiter) : IRequestHandler<DeleteDepartmentCommand, ApiResponse>

{
    public async Task<ApiResponse> Handle(DeleteDepartmentCommand request, CancellationToken cancellationToken)
    {
        var getDepartment = await commiter.Departments.GetAsync(x => x.Id == request.departmentId,
            QueryIncludeHelper.IncludeDepartmentRelations());
        if(getDepartment.IsSuccess is false ||getDepartment.Data is null)
        {
            return ApiResponse.Failure(HttpStatusCode.NotFound,getDepartment.Message?? "no department was found to delete");
        }
        var department = getDepartment.Data;
        if(department.SubDepartments is not null && department.SubDepartments.Count > 0)
        {
            department.SubDepartments.Clear();
            var dbResult = await commiter.Departments.UpdateAsync(department);
            if (dbResult.IsSuccess is false) 
            {
                return dbResult;
            }
            var commiterResult = await commiter.CommitAsync(cancellationToken);
            if (commiterResult == 0)
                return ApiResponse.Failure(HttpStatusCode.InternalServerError,"error Accord while saving changes");
        }
        var removeResult = await commiter.Departments.RemoveAsync(x => x.Id == department.Id);
        var deleteCommitResult= await commiter.CommitAsync(cancellationToken);
        if(deleteCommitResult == 0)
        {
            return ApiResponse.Failure(HttpStatusCode.InternalServerError, "Error accourd while save changes");
        }
        return removeResult;
    }
}
