using Microsoft.AspNetCore.Http;

namespace TMS.Contract.CQRS.Commands.CustomCommands.WorkTaskCommands.Dtos;

public class SubmitTaskRequestDto
{
    public Guid TaskGuid { get; set; }
    public string Comment { get; set; } = string.Empty;
    public ICollection<IFormFile> Files = [];
}
