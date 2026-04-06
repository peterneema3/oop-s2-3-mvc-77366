using Microsoft.EntityFrameworkCore;
using VgcCollege.Web.Data;

namespace VgcCollege.Web.Services
{
    public class ResultService
    {
        private readonly ApplicationDbContext _context;

        public ResultService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> IsValidAssignmentScoreAsync(int assignmentId, double score)
        {
            var assignment = await _context.Assignments.FindAsync(assignmentId);
            return assignment != null && score >= 0 && score <= assignment.MaxScore;
        }

        public async Task<bool> IsValidExamScoreAsync(int examId, double score)
        {
            var exam = await _context.Exams.FindAsync(examId);
            return exam != null && score >= 0 && score <= exam.MaxScore;
        }

        public string CalculateGrade(double score)
        {
            if (score >= 70) return "A";
            if (score >= 60) return "B";
            if (score >= 50) return "C";
            if (score >= 40) return "D";
            return "F";
        }
    }
}