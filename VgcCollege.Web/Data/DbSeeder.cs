using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using VgcCollege.Web.Models;

namespace VgcCollege.Web.Data
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(IServiceProvider services)
        {
            var context = services.GetRequiredService<ApplicationDbContext>();
            var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

            await context.Database.MigrateAsync();

            string[] roles = { "Administrator", "Faculty", "Student" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));
            }

            async Task<ApplicationUser> CreateUserAsync(string email, string password, string role)
            {
                var user = await userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    user = new ApplicationUser
                    {
                        UserName = email,
                        Email = email,
                        EmailConfirmed = true
                    };

                    await userManager.CreateAsync(user, password);
                    await userManager.AddToRoleAsync(user, role);
                }

                return user;
            }

            var adminUser = await CreateUserAsync("admin@vgc.com", "Password1!", "Administrator");
            var facultyUser = await CreateUserAsync("faculty@vgc.com", "Password1!", "Faculty");
            var student1User = await CreateUserAsync("student1@vgc.com", "Password1!", "Student");
            var student2User = await CreateUserAsync("student2@vgc.com", "Password1!", "Student");

            if (!context.Branches.Any())
            {
                context.Branches.AddRange(
                    new Branch { Name = "Dublin Branch", Address = "Dublin City" },
                    new Branch { Name = "Cork Branch", Address = "Cork City" },
                    new Branch { Name = "Galway Branch", Address = "Galway City" }
                );
                await context.SaveChangesAsync();
            }

            if (!context.Courses.Any())
            {
                var dublin = await context.Branches.FirstAsync();
                context.Courses.AddRange(
                    new Course
                    {
                        Name = "Software Development",
                        BranchId = dublin.Id,
                        StartDate = DateTime.Today,
                        EndDate = DateTime.Today.AddMonths(9)
                    },
                    new Course
                    {
                        Name = "Business Information Systems",
                        BranchId = dublin.Id,
                        StartDate = DateTime.Today,
                        EndDate = DateTime.Today.AddMonths(9)
                    }
                );
                await context.SaveChangesAsync();
            }

            if (!context.FacultyProfiles.Any())
            {
                context.FacultyProfiles.Add(new FacultyProfile
                {
                    IdentityUserId = facultyUser.Id,
                    Name = "John Faculty",
                    Email = facultyUser.Email!,
                    Phone = "111111111"
                });
                await context.SaveChangesAsync();
            }

            if (!context.StudentProfiles.Any())
            {
                context.StudentProfiles.AddRange(
                    new StudentProfile
                    {
                        IdentityUserId = student1User.Id,
                        Name = "Alice Student",
                        Email = student1User.Email!,
                        Phone = "222222222",
                        Address = "Dublin",
                        StudentNumber = "S001",
                        DOB = new DateTime(2001, 1, 1)
                    },
                    new StudentProfile
                    {
                        IdentityUserId = student2User.Id,
                        Name = "Bob Student",
                        Email = student2User.Email!,
                        Phone = "333333333",
                        Address = "Cork",
                        StudentNumber = "S002",
                        DOB = new DateTime(2002, 2, 2)
                    }
                );
                await context.SaveChangesAsync();
            }

            if (!context.FacultyCourses.Any())
            {
                var faculty = await context.FacultyProfiles.FirstAsync();
                var course = await context.Courses.FirstAsync();

                context.FacultyCourses.Add(new FacultyCourse
                {
                    FacultyProfileId = faculty.Id,
                    CourseId = course.Id
                });

                await context.SaveChangesAsync();
            }

            if (!context.CourseEnrolments.Any())
            {
                var course = await context.Courses.FirstAsync();
                var students = await context.StudentProfiles.ToListAsync();

                context.CourseEnrolments.AddRange(
                    new CourseEnrolment
                    {
                        StudentProfileId = students[0].Id,
                        CourseId = course.Id,
                        EnrolDate = DateTime.Today,
                        Status = "Active"
                    },
                    new CourseEnrolment
                    {
                        StudentProfileId = students[1].Id,
                        CourseId = course.Id,
                        EnrolDate = DateTime.Today,
                        Status = "Active"
                    }
                );

                await context.SaveChangesAsync();
            }

            if (!context.Assignments.Any())
            {
                var course = await context.Courses.FirstAsync();
                context.Assignments.Add(new Assignment
                {
                    CourseId = course.Id,
                    Title = "OOP Assignment 1",
                    MaxScore = 100,
                    DueDate = DateTime.Today.AddDays(14)
                });
                await context.SaveChangesAsync();
            }

            if (!context.Exams.Any())
            {
                var course = await context.Courses.FirstAsync();
                context.Exams.AddRange(
                    new Exam
                    {
                        CourseId = course.Id,
                        Title = "Midterm Exam",
                        Date = DateTime.Today.AddDays(21),
                        MaxScore = 100,
                        ResultsReleased = false
                    },
                    new Exam
                    {
                        CourseId = course.Id,
                        Title = "Final Exam",
                        Date = DateTime.Today.AddDays(60),
                        MaxScore = 100,
                        ResultsReleased = true
                    }
                );
                await context.SaveChangesAsync();
            }
        }
    }
}