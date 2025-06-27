using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;

namespace TMS.Contract.CQRS.Commands.CustomCommands.WorkTaskCommands.Dtos;

public class SubmitTaskRequestDto
{
    public string Description { get; set; } = string.Empty;
    public ICollection<IFormFile>? Files { get; set; } = [];
}
