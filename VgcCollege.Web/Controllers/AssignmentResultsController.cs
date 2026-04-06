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
    public class AssignmentResultsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ResultService _resultService;
        private readonly FacultyAccessService _facultyAccessService;

        public AssignmentResultsController(
            ApplicationDbContext context,
            ResultService resultService,
            FacultyAccessService facultyAccessService)
        {
            _context = context;
            _resultService = resultService;
            _facultyAccessService = facultyAccessService;
        }

        public async Task<IActionResult> Index()
        {
            var results = await _context.AssignmentResults
                .Include(r => r.Assignment)
                .Include(r => r.StudentProfile)
                .ToListAsync();

            return View(results);
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.AssignmentId = new SelectList(await _context.Assignments.ToListAsync(), "Id", "Title");
            ViewBag.StudentProfileId = new SelectList(await _context.StudentProfiles.ToListAsync(), "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AssignmentResult result)
        {
            if (!await _resultService.IsValidAssignmentScoreAsync(result.AssignmentId, result.Score))
                ModelState.AddModelError("Score", "Score must be between 0 and assignment max score.");

            if (User.IsInRole("Faculty"))
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value!;
                var assignment = await _context.Assignments.FirstAsync(a => a.Id == result.AssignmentId);
                bool allowed = await _facultyAccessService.FacultyCanAccessCourseAsync(userId, assignment.CourseId);
                if (!allowed)
                    return Forbid();
            }

            if (ModelState.IsValid)
            {
                _context.AssignmentResults.Add(result);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.AssignmentId = new SelectList(await _context.Assignments.ToListAsync(), "Id", "Title", result.AssignmentId);
            ViewBag.StudentProfileId = new SelectList(await _context.StudentProfiles.ToListAsync(), "Id", "Name", result.StudentProfileId);
            return View(result);
        }
    }
}