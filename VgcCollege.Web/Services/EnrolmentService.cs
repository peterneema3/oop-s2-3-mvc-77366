using Microsoft.EntityFrameworkCore;
using VgcCollege.Web.Data;
using VgcCollege.Web.Models;

namespace VgcCollege.Web.Services
{
    public class EnrolmentService
    {
        private readonly ApplicationDbContext _context;

        public EnrolmentService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> CanEnrolAsync(int studentProfileId, int courseId)
        {
            return !await _context.CourseEnrolments
                .AnyAsync(e => e.StudentProfileId == studentProfileId && e.CourseId == courseId);
        }

        public async Task<(bool Success, string Message)> EnrolAsync(int studentProfileId, int courseId)
        {
            if (!await CanEnrolAsync(studentProfileId, courseId))
                return (false, "Student is already enrolled in this course.");

            var enrolment = new CourseEnrolment
            {
                StudentProfileId = studentProfileId,
                CourseId = courseId,
                EnrolDate = DateTime.Today,
                Status = "Active"
            };

            _context.CourseEnrolments.Add(enrolment);
            await _context.SaveChangesAsync();

            return (true, "Student enrolled successfully.");
        }
    }
}