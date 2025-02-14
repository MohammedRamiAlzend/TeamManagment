using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Data.Entities
{
    public class WorkTask : IHasId, IAuditable, ISoftDeletable
    {
        public int Id { get; set; }
        //Should be unique
        //TODO:make an interceptor and check if the type then generate it and
        //save it in database
        public string TaskUniqueIdentifier { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public string Status { get; set; }
        public string? Priority { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        [NotMapped]
        public DateTime? DeadLine { get; set; }
        public int PointId {  get; set; }
        public Point Point { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
