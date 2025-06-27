using TMS.Contract.CQRS.Commands.CustomCommands.ProjectCommands.Dtos;

namespace TMS.Application.Services.Interfaces.ProjectInterfaces;

public interface IProjectValidator
{
    Task<DbRequest> ValidateAdd(AddProjectDto dto);
} 