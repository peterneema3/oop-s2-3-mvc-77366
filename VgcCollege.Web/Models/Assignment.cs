using System.ComponentModel.DataAnnotations;

namespace VgcCollege.Web.Models
{
    public class Assignment
    {
        public int Id { get; set; }

        [Required]
        public int CourseId { get; set; }
        public Course? Course { get; set; }

        [Required, StringLength(150)]
        public string Title { get; set; } = string.Empty;

        [Range(1, 1000)]
        public int MaxScore { get; set; }

        [DataType(DataType.Date)]
        public DateTime DueDate { get; set; }

        public ICollection<AssignmentResult> Results { get; set; } = new List<AssignmentResult>();
    }
}