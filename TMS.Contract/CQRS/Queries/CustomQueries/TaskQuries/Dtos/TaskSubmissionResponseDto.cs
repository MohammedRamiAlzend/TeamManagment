using System;
using System.Collections.Generic;
using TMS.Contract.CQRS.Queries.CustomQueries.TaskQuries.Dtos;
using TMS.Contract.CQRS.Queries.CustomQueries.EmployeeQuries;
using TMS.Contract.CQRS.Queries.CustomQueries.TaskQuries;
using TMS.Contract.Entities.Interfaces;

namespace TMS.Contract.CQRS.Queries.CustomQueries.TaskQuries.Dtos;

public class TaskSubmissionResponseDto : IDto
{
    public Guid SubmissionUniqueIdentifier { get; set; }
    public Guid WorkTaskGuid { get; set; }
    public int SubmittedByEmployeeId { get; set; }
    public DateTime SubmissionDate { get; set; }
    public string Description { get; set; }
    public string Status { get; set; }
    public DateTime? ReviewedDate { get; set; }
    public string FeedbackComments { get; set; }
    public List<Guid> FileIds { get; set; }
} 