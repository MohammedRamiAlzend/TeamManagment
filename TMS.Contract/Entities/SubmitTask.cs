using TMS.Contract.Entities.Enums;

namespace TMS.Contract.Entities;

public class SubmitTask : Entity
{
    public int TaskId { get; set; }
    public Task Task { get; set; }
    public string? SubmitContent { get; set; }
    public DateTime SubmitDate { get; set; }
}