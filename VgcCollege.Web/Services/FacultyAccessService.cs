using Microsoft.EntityFrameworkCore;
using VgcCollege.Web.Data;

namespace VgcCollege.Web.Services
{
    public class FacultyAccessService
    {
        private readonly ApplicationDbContext _context;

        public FacultyAccessService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> FacultyCanAccessCourseAsync(string userId, int courseId)
        {
            var faculty = await _context.FacultyProfiles
                .FirstOrDefaultAsync(f => f.IdentityUserId == userId);

            if (faculty == null) return false;

            return await _context.FacultyCourses
                .AnyAsync(fc => fc.FacultyProfileId == faculty.Id && fc.CourseId == courseId);
        }
    }
}