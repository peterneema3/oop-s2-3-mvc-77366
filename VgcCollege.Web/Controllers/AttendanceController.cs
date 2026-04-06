using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VgcCollege.Web.Data;
using VgcCollege.Web.Models;
using VgcCollege.Web.Services;

namespace VgcCollege.Web.Controllers
{
    [Authorize(Roles = "Administrator,Faculty")]
    public class AttendanceController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly FacultyAccessService _facultyAccessService;

        public AttendanceController(ApplicationDbContext context, FacultyAccessService facultyAccessService)
        {
            _context = context;
            _facultyAccessService = facultyAccessService;
        }

        public async Task<IActionResult> Index()
        {
            var records = await _context.AttendanceRecords
                .Include(a => a.CourseEnrolment)!
                    .ThenInclude(e => e.StudentProfile)
                .Include(a => a.CourseEnrolment)!
                    .ThenInclude(e => e.Course)
                .ToListAsync();

            return View(records);
        }

        public async Task<IActionResult> Create()
        {
            var enrolments = await _context.CourseEnrolments
                .Include(e => e.StudentProfile)
                .Include(e => e.Course)
                .ToListAsync();

            ViewBag.CourseEnrolmentId = new SelectList(
                enrolments.Select(e => new
                {
                    e.Id,
                    Display = $"{e.StudentProfile!.Name} - {e.Course!.Name}"
                }),
                "Id",
                "Display");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AttendanceRecord record)
        {
            if (User.IsInRole("Faculty"))
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value!;
                var enrolment = await _context.CourseEnrolments
                    .Include(e => e.Course)
                    .FirstAsync(e => e.Id == record.CourseEnrolmentId);

                bool allowed = await _facultyAccessService.FacultyCanAccessCourseAsync(userId, enrolment.CourseId);
                if (!allowed)
                    return Forbid();
            }

            if (ModelState.IsValid)
            {
                _context.AttendanceRecords.Add(record);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(record);
        }
    }
}