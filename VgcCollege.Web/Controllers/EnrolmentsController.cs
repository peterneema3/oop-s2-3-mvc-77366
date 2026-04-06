using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VgcCollege.Web.Data;
using VgcCollege.Web.Services;

namespace VgcCollege.Web.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class EnrolmentsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly EnrolmentService _enrolmentService;

        public EnrolmentsController(ApplicationDbContext context, EnrolmentService enrolmentService)
        {
            _context = context;
            _enrolmentService = enrolmentService;
        }

        public async Task<IActionResult> Index()
        {
            var enrolments = await _context.CourseEnrolments
                .Include(e => e.StudentProfile)
                .Include(e => e.Course)
                .ToListAsync();

            return View(enrolments);
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.StudentProfileId = new SelectList(await _context.StudentProfiles.ToListAsync(), "Id", "Name");
            ViewBag.CourseId = new SelectList(await _context.Courses.ToListAsync(), "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int studentProfileId, int courseId)
        {
            var result = await _enrolmentService.EnrolAsync(studentProfileId, courseId);

            if (!result.Success)
            {
                ModelState.AddModelError("", result.Message);
                ViewBag.StudentProfileId = new SelectList(await _context.StudentProfiles.ToListAsync(), "Id", "Name", studentProfileId);
                ViewBag.CourseId = new SelectList(await _context.Courses.ToListAsync(), "Id", "Name", courseId);
                return View();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}