using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VgcCollege.Web.Data;

namespace VgcCollege.Web.Controllers
{
    [Authorize(Roles = "Faculty")]
    public class FacultyPortalController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FacultyPortalController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Dashboard()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
                return Challenge();

            var faculty = await _context.FacultyProfiles
                .FirstOrDefaultAsync(f => f.IdentityUserId == userId);

            if (faculty == null)
                return NotFound();

            var courseIds = await _context.FacultyCourses
                .Where(fc => fc.FacultyProfileId == faculty.Id)
                .Select(fc => fc.CourseId)
                .ToListAsync();

            ViewBag.Courses = await _context.Courses
                .Where(c => courseIds.Contains(c.Id))
                .ToListAsync();

            ViewBag.Enrolments = await _context.CourseEnrolments
                .Include(e => e.StudentProfile)
                .Include(e => e.Course)
                .Where(e => courseIds.Contains(e.CourseId))
                .ToListAsync();

            return View();
        }
    }
}