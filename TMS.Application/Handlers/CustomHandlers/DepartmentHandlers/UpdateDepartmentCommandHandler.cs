using System.Text;
using Bogus.DataSets;
using TMS.Application.Services.Interfaces.DepartmentInterfaces;
using TMS.Application.Services.Interfaces.EmployeeInterfaces;

namespace TMS.Application.Handlers.CustomHandlers.DepartmentHandlers;

public class UpdateDepartmentCommandHandler : IRequestHandler<UpdateDepartmentCommand, ApiResponse>
{
    private readonly IDepartmentValidator _validator;
    private readonly IEmployeeService _employeeService;
    private readonly IEntityCommiter _commiter;
    private readonly ILogger<UpdateDepartmentCommandHandler> _logger;

    public UpdateDepartmentCommandHandler(
        IDepartmentValidator validator,
        IEmployeeService employeeService,
        IEntityCommiter commiter,
        ILogger<UpdateDepartmentCommandHandler> logger)
    {
        _validator = validator;
        _employeeService = employeeService;
        _commiter = commiter;
        _logger = logger;
    }

    public async Task<ApiResponse> Handle(UpdateDepartmentCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Update Department Command Handler is running");
        _logger.LogInformation("Checking request informations ......");
        var validationResult = await _validator.ValidateUpdate(request.departmentId, request.Dto);
        if (!validationResult.IsSuccess)
        {
            return validationResult;
        }
        _logger.LogInformation("informations Accepted ....");
        _logger.LogInformation("start Updating department ....");
        var updateResult = await _employeeService.UpdateDepartment(request.departmentId, request.Dto, cancellationToken);
        return updateResult.IsSuccess 
            ? ApiResponse.Success(HttpStatusCode.OK, $"department {request.Dto.Name} has been updated successfully")
            : ApiResponse.Failure(HttpStatusCode.BadRequest, updateResult.Message!);
    }
}
