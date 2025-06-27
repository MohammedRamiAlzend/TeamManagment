using System;
using TMS.Contract.Entities;
using TMS.Contract.CommunicationModels;

namespace TMS.Contract.CQRS.Commands.CustomCommands.WorkTaskCommands;

public record DeleteTaskCommand(Guid TaskGuidId) : IRequest<ApiResponse>; 