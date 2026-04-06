using VgcCollege.Web.Models;
using VgcCollege.Web.Services;
using Xunit;

namespace VgcCollege.Tests
{
    public class EnrolmentServiceTests
    {
        [Fact]
        public async Task CanEnrol_ReturnsTrue_WhenStudentNotAlreadyEnrolled()
        {
            using var context = TestDbHelper.GetDbContext();
            var service = new EnrolmentService(context);

            var result = await service.CanEnrolAsync(1, 1);

            Assert.True(result);
        }

        [Fact]
        public async Task CanEnrol_ReturnsFalse_WhenStudentAlreadyEnrolled()
        {
            using var context = TestDbHelper.GetDbContext();
            context.CourseEnrolments.Add(new CourseEnrolment
            {
                StudentProfileId = 1,
                CourseId = 1,
                Status = "Active"
            });
            await context.SaveChangesAsync();

            var service = new EnrolmentService(context);

            var result = await service.CanEnrolAsync(1, 1);

            Assert.False(result);
        }

        [Fact]
        public async Task EnrolAsync_CreatesNewEnrolment_WhenValid()
        {
            using var context = TestDbHelper.GetDbContext();
            var service = new EnrolmentService(context);

            var result = await service.EnrolAsync(1, 2);

            Assert.True(result.Success);
            Assert.Single(context.CourseEnrolments);
        }
    }
}