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
    public class ExamResultsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ResultService _resultService;
        private readonly FacultyAccessService _facultyAccessService;

        public ExamResultsController(
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
            var results = await _context.ExamResults
                .Include(r => r.Exam)
                .Include(r => r.StudentProfile)
                .ToListAsync();

            return View(results);
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.ExamId = new SelectList(await _context.Exams.ToListAsync(), "Id", "Title");
            ViewBag.StudentProfileId = new SelectList(await _context.StudentProfiles.ToListAsync(), "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ExamResult result)
        {
            if (!await _resultService.IsValidExamScoreAsync(result.ExamId, result.Score))
                ModelState.AddModelError("Score", "Score must be between 0 and exam max score.");

            result.Grade = _resultService.CalculateGrade(result.Score);

            if (User.IsInRole("Faculty"))
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value!;
                var exam = await _context.Exams.FirstAsync(e => e.Id == result.ExamId);
                bool allowed = await _facultyAccessService.FacultyCanAccessCourseAsync(userId, exam.CourseId);
                if (!allowed)
                    return Forbid();
            }

            if (ModelState.IsValid)
            {
                _context.ExamResults.Add(result);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.ExamId = new SelectList(await _context.Exams.ToListAsync(), "Id", "Title", result.ExamId);
            ViewBag.StudentProfileId = new SelectList(await _context.StudentProfiles.ToListAsync(), "Id", "Name", result.StudentProfileId);
            return View(result);
        }
    }
}