namespace TMS.Contract.CQRS.Commands.CustomCommands.WorkTaskCommands.Dtos;

public class SubmitTaskResponseDto : IDto
{
    public required string Name { get; set; }
    public required string ContentType { get; set; }
    public required long Length { get; set; }
    public required string FileName { get; set; }
}