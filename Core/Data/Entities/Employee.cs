using System.ComponentModel.DataAnnotations.Schema;
using Core.Data.Entities.Interfaces;

namespace Core.Data.Entities
{
    public class Employee : IHasId, ISoftDeletable, IAuditable
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? FatherName { get; set; }
        public string? MiddleName { get; set; }
        public string? MotherName { get; set; }
        public string NationalIdentificationNumber { get; set; }
        public DateTime BirthDate { get; set; }
        public DateTime HireDate { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public ICollection<Department> Departments { get; set; }
        public ICollection<Role> EmployeesRoles { get; set; }
        public ICollection<TaskAssignment>? AssignedToEmployeeTasks { get; set; }
        public ICollection<TaskAssignment>? AssignedByEmployeeTasks { get; set; }


        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}