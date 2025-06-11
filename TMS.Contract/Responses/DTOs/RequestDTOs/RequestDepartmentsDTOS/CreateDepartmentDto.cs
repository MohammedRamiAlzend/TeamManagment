using System.ComponentModel.DataAnnotations;

namespace TMS.Contract.Responses.DTOs.RequestDTOs.RequestDepartmentsDTOS;

public class CreateDepartmentDto : IDto
{
    [Required] public string Name { get; set; }

    [Required] public string Email { get; set; }

    [Required] public string PhoneNumber { get; set; }

    public int? ParentDepartmentId { get; set; }

    [Required] public int? TeamLeaderId { get; set; }
}