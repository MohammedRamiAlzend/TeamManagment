using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMS.Core.AutoMapperClasses.DTOs.ResponseDTOs
{
    public class EmployeeDto : IDto
    {
        public int Id { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public ICollection<string>? Departments { get; set; } // Simplified department names
    }
}
