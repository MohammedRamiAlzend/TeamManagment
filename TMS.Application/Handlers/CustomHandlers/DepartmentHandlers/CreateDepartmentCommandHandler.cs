using System.Text;
using TMS.Application.Services.Interfaces.DepartmentInterfaces;

namespace TMS.Application.Handlers.CustomHandlers.DepartmentHandlers;

public class CreateDepartmentCommandHandler : IRequestHandler<CreateDepartmentCommand, ApiResponse>
{
    private readonly ICreateDepartmentValidator _validator;
    private readonly ICreateDepartmentService _service;
    private readonly ILogger<CreateDepartmentCommandHandler> _logger;

    public CreateDepartmentCommandHandler(ICreateDepartmentValidator validator, ICreateDepartmentService service, ILogger<CreateDepartmentCommandHandler> logger)
    {
        _validator = validator;
        _service = service;
        _logger = logger;
    }

    public async Task<ApiResponse> Handle(CreateDepartmentCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Create Department Command Handler is running");
        _logger.LogInformation("Checking request informations ......");
        var validationResult = await _validator.Validate(request.Dto);
        if (!validationResult.IsSuccess)
        {
            return validationResult;
        }
        _logger.LogInformation("informations Accepted ....");

        var createDepartmentResult = await _service.CreateDepartment(request.Dto, cancellationToken);

        return createDepartmentResult.IsSuccess
            ? ApiResponse.Success(HttpStatusCode.OK, $"department {request.Dto.Name} has been added successfully")
            : ApiResponse.Failure(HttpStatusCode.BadRequest, createDepartmentResult.Message!);
    }
}