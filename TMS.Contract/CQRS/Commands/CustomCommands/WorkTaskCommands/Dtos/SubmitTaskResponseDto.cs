namespace TMS.Contract.CQRS.Commands.CustomCommands.WorkTaskCommands.Dtos;

public class SubmitTaskResponseDto : IDto
{
    public required Guid FileGuidId { get; set; }
    public required string Name { get; set; }
    public required string ContentType { get; set; }
    public required long Length { get; set; }
    public required string FileName { get; set; }
}