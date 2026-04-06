using VgcCollege.Web.Models;
using VgcCollege.Web.Services;
using Xunit;

namespace VgcCollege.Tests
{
    public class FacultyAccessServiceTests
    {
        [Fact]
        public async Task FacultyCanAccessCourseAsync_ReturnsTrue_WhenAssigned()
        {
            using var context = TestDbHelper.GetDbContext();

            context.FacultyProfiles.Add(new FacultyProfile
            {
                Id = 1,
                IdentityUserId = "faculty-1",
                Name = "Faculty User",
                Email = "faculty@test.com"
            });

            context.FacultyCourses.Add(new FacultyCourse
            {
                FacultyProfileId = 1,
                CourseId = 10
            });

            await context.SaveChangesAsync();

            var service = new FacultyAccessService(context);

            var result = await service.FacultyCanAccessCourseAsync("faculty-1", 10);

            Assert.True(result);
        }

        [Fact]
        public async Task FacultyCanAccessCourseAsync_ReturnsFalse_WhenNotAssigned()
        {
            using var context = TestDbHelper.GetDbContext();

            context.FacultyProfiles.Add(new FacultyProfile
            {
                Id = 1,
                IdentityUserId = "faculty-1",
                Name = "Faculty User",
                Email = "faculty@test.com"
            });

            await context.SaveChangesAsync();

            var service = new FacultyAccessService(context);

            var result = await service.FacultyCanAccessCourseAsync("faculty-1", 10);

            Assert.False(result);
        }
    }
}