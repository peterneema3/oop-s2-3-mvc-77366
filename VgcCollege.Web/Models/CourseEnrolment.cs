using System.ComponentModel.DataAnnotations;

namespace VgcCollege.Web.Models
{
    public class CourseEnrolment
    {
        public int Id { get; set; }

        [Required]
        public int StudentProfileId { get; set; }
        public StudentProfile? StudentProfile { get; set; }

        [Required]
        public int CourseId { get; set; }
        public Course? Course { get; set; }

        [DataType(DataType.Date)]
        public DateTime EnrolDate { get; set; } = DateTime.UtcNow;

        [Required]
        public string Status { get; set; } = "Active";

        public ICollection<AttendanceRecord> AttendanceRecords { get; set; } = new List<AttendanceRecord>();
    }
}