using VgcCollege.Web.Models;
using VgcCollege.Web.Services;
using Xunit;

namespace VgcCollege.Tests
{
    public class ResultServiceTests
    {
        [Fact]
        public async Task IsValidAssignmentScoreAsync_ReturnsTrue_WhenWithinMax()
        {
            using var context = TestDbHelper.GetDbContext();
            context.Assignments.Add(new Assignment { Id = 1, Title = "A1", MaxScore = 100 });
            await context.SaveChangesAsync();

            var service = new ResultService(context);

            Assert.True(await service.IsValidAssignmentScoreAsync(1, 80));
        }

        [Fact]
        public async Task IsValidAssignmentScoreAsync_ReturnsFalse_WhenAboveMax()
        {
            using var context = TestDbHelper.GetDbContext();
            context.Assignments.Add(new Assignment { Id = 1, Title = "A1", MaxScore = 100 });
            await context.SaveChangesAsync();

            var service = new ResultService(context);

            Assert.False(await service.IsValidAssignmentScoreAsync(1, 120));
        }

        [Theory]
        [InlineData(75, "A")]
        [InlineData(65, "B")]
        [InlineData(55, "C")]
        [InlineData(45, "D")]
        [InlineData(20, "F")]
        public void CalculateGrade_ReturnsExpectedGrade(double score, string expected)
        {
            using var context = TestDbHelper.GetDbContext();
            var service = new ResultService(context);

            Assert.Equal(expected, service.CalculateGrade(score));
        }
    }
}