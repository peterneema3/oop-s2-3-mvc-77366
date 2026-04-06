using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VgcCollege.Web.Data;

namespace VgcCollege.Web.Controllers
{
    [Authorize(Roles = "Student")]
    public class StudentPortalController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StudentPortalController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Dashboard()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value!;

            var student = await _context.StudentProfiles
                .FirstOrDefaultAsync(s => s.IdentityUserId == userId);

            if (student == null) return NotFound();

            ViewBag.Student = student;

            ViewBag.Enrolments = await _context.CourseEnrolments
                .Include(e => e.Course)
                .Where(e => e.StudentProfileId == student.Id)
                .ToListAsync();

            ViewBag.AssignmentResults = await _context.AssignmentResults
                .Include(r => r.Assignment)
                .Where(r => r.StudentProfileId == student.Id)
                .ToListAsync();

            ViewBag.ExamResults = await _context.ExamResults
                .Include(r => r.Exam)
                .Where(r => r.StudentProfileId == student.Id && r.Exam!.ResultsReleased)
                .ToListAsync();

            return View();
        }
    }
}