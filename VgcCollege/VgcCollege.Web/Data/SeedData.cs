using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using VgcCollege.Web.Models;

namespace VgcCollege.Web.Data;

public static class SeedData
{
    public static async Task InitializeAsync(IServiceProvider serviceProvider)
    {
        using var context = new ApplicationDbContext(
            serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>());
        
        var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        
        await context.Database.EnsureCreatedAsync();
        
        string[] roles = { "Admin", "Faculty", "Student" };
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }
        
        var adminEmail = "admin@vgc.edu";
        if (await userManager.FindByEmailAsync(adminEmail) == null)
        {
            var admin = new IdentityUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true
            };
            await userManager.CreateAsync(admin, "Admin@123");
            await userManager.AddToRoleAsync(admin, "Admin");
        }
        
        var facultyEmail = "faculty@vgc.edu";
        IdentityUser? facultyUser = null;
        if (await userManager.FindByEmailAsync(facultyEmail) == null)
        {
            facultyUser = new IdentityUser
            {
                UserName = facultyEmail,
                Email = facultyEmail,
                EmailConfirmed = true
            };
            await userManager.CreateAsync(facultyUser, "Faculty@123");
            await userManager.AddToRoleAsync(facultyUser, "Faculty");
        }
        else
        {
            facultyUser = await userManager.FindByEmailAsync(facultyEmail);
        }
        
        var studentEmails = new[] { "student1@vgc.edu", "student2@vgc.edu" };
        var studentUsers = new List<IdentityUser>();
        
        foreach (var email in studentEmails)
        {
            var student = await userManager.FindByEmailAsync(email);
            if (student == null)
            {
                student = new IdentityUser
                {
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(student, "Student@123");
                await userManager.AddToRoleAsync(student, "Student");
                studentUsers.Add(student);
            }
            else
            {
                studentUsers.Add(student);
            }
        }
        
        if (!context.Branches.Any())
        {
            context.Branches.AddRange(
                new Branch { Name = "Dublin City Centre", Address = "1 Main Street, Dublin 1" },
                new Branch { Name = "Cork Campus", Address = "45 South Mall, Cork" },
                new Branch { Name = "Galway Campus", Address = "12 Shop Street, Galway" }
            );
            await context.SaveChangesAsync();
        }
        
        if (facultyUser != null && !context.FacultyProfiles.Any())
        {
            context.FacultyProfiles.Add(new FacultyProfile
            {
                IdentityUserId = facultyUser.Id,
                Name = "Dr. John Smith",
                Email = facultyUser.Email ?? "faculty@vgc.edu",
                Phone = "+353 1 234 5678"
            });
            await context.SaveChangesAsync();
        }
        
        if (!context.StudentProfiles.Any())
        {
            var student1 = studentUsers[0];
            var student2 = studentUsers[1];
            
            context.StudentProfiles.AddRange(
                new StudentProfile
                {
                    IdentityUserId = student1.Id,
                    StudentNumber = "STU2024001",
                    Name = "Bill Johnson",
                    Email = student1.Email ?? "student1@vgc.edu",
                    Phone = "087 123 4567",
                    Address = "15 O'Connell Street, Dublin",
                    DateOfBirth = new DateTime(2000, 5, 15)
                },
                new StudentProfile
                {
                    IdentityUserId = student2.Id,
                    StudentNumber = "STU2024002",
                    Name = "Adam Smith",
                    Email = student2.Email ?? "student2@vgc.edu",
                    Phone = "087 765 4321",
                    Address = "22 Patrick Street, Cork",
                    DateOfBirth = new DateTime(2001, 8, 22)
                }
            );
            await context.SaveChangesAsync();
        }
        
        if (!context.Courses.Any())
        {
            var dublinBranch = await context.Branches.FirstAsync(b => b.Name.Contains("Dublin"));
            var corkBranch = await context.Branches.FirstAsync(b => b.Name.Contains("Cork"));
            
            context.Courses.AddRange(
                new Course
                {
                    Name = "Computer Science - Year 1",
                    BranchId = dublinBranch.Id,
                    StartDate = new DateTime(2024, 9, 1),
                    EndDate = new DateTime(2025, 5, 31)
                },
                new Course
                {
                    Name = "Business Administration",
                    BranchId = dublinBranch.Id,
                    StartDate = new DateTime(2024, 9, 1),
                    EndDate = new DateTime(2025, 5, 31)
                },
                new Course
                {
                    Name = "Software Development",
                    BranchId = corkBranch.Id,
                    StartDate = new DateTime(2024, 9, 1),
                    EndDate = new DateTime(2025, 5, 31)
                }
            );
            await context.SaveChangesAsync();
        }
        
        if (!context.CourseEnrolments.Any())
        {
            var csCourse = await context.Courses.FirstAsync(c => c.Name.Contains("Computer Science"));
            var bill = await context.StudentProfiles.FirstAsync(s => s.Name.Contains("Bill"));
            var adam = await context.StudentProfiles.FirstAsync(s => s.Name.Contains("Adam"));
            
            context.CourseEnrolments.AddRange(
                new CourseEnrolment
                {
                    StudentProfileId = bill.Id,
                    CourseId = csCourse.Id,
                    EnrolDate = DateTime.Now.AddMonths(-2),
                    Status = "Active"
                },
                new CourseEnrolment
                {
                    StudentProfileId = adam.Id,
                    CourseId = csCourse.Id,
                    EnrolDate = DateTime.Now.AddMonths(-2),
                    Status = "Active"
                }
            );
            await context.SaveChangesAsync();
        }
        
        if (!context.FacultyCourses.Any())
        {
            var facultyProfile = await context.FacultyProfiles.FirstOrDefaultAsync();
            var csCourse = await context.Courses.FirstAsync(c => c.Name.Contains("Computer Science"));
            
            if (facultyProfile != null)
            {
                context.FacultyCourses.Add(new FacultyCourse
                {
                    FacultyProfileId = facultyProfile.Id,
                    CourseId = csCourse.Id
                });
                await context.SaveChangesAsync();
            }
        }
        
        if (!context.Assignments.Any())
        {
            var csCourse = await context.Courses.FirstAsync(c => c.Name.Contains("Computer Science"));
            
            context.Assignments.AddRange(
                new Assignment
                {
                    CourseId = csCourse.Id,
                    Title = "Programming Fundamentals - Project",
                    MaxScore = 100,
                    DueDate = DateTime.Now.AddDays(14)
                },
                new Assignment
                {
                    CourseId = csCourse.Id,
                    Title = "Data Structures - Assignment",
                    MaxScore = 100,
                    DueDate = DateTime.Now.AddDays(21)
                }
            );
            await context.SaveChangesAsync();
        }
        
        if (!context.AssignmentResults.Any())
        {
            var assignment = await context.Assignments.FirstAsync();
            var bill = await context.StudentProfiles.FirstAsync(s => s.Name.Contains("Bill"));
            
            context.AssignmentResults.Add(new AssignmentResult
            {
                AssignmentId = assignment.Id,
                StudentProfileId = bill.Id,
                Score = 85,
                Feedback = "Good work! Keep it up."
            });
            await context.SaveChangesAsync();
        }
        
        if (!context.Exams.Any())
        {
            var csCourse = await context.Courses.FirstAsync(c => c.Name.Contains("Computer Science"));
            
            context.Exams.AddRange(
                new Exam
                {
                    CourseId = csCourse.Id,
                    Title = "Mid-Term Exam",
                    Date = DateTime.Now.AddDays(30),
                    MaxScore = 100,
                    ResultsReleased = false
                },
                new Exam
                {
                    CourseId = csCourse.Id,
                    Title = "Final Exam",
                    Date = DateTime.Now.AddDays(60),
                    MaxScore = 100,
                    ResultsReleased = false
                }
            );
            await context.SaveChangesAsync();
        }
        
        if (!context.ExamResults.Any())
        {
            var midTermExam = await context.Exams.FirstAsync(e => e.Title.Contains("Mid-Term"));
            var bill = await context.StudentProfiles.FirstAsync(s => s.Name.Contains("Bill"));
            var adam = await context.StudentProfiles.FirstAsync(s => s.Name.Contains("Adam"));
            
            context.ExamResults.AddRange(
                new ExamResult
                {
                    ExamId = midTermExam.Id,
                    StudentProfileId = bill.Id,
                    Score = 78,
                    Grade = "B+"
                },
                new ExamResult
                {
                    ExamId = midTermExam.Id,
                    StudentProfileId = adam.Id,
                    Score = 82,
                    Grade = "A-"
                }
            );
            await context.SaveChangesAsync();
        }
        
        if (!context.AttendanceRecords.Any())
        {
            var billEnrolment = await context.CourseEnrolments
                .FirstAsync(e => e.StudentProfile.Name.Contains("Bill"));
            var adamEnrolment = await context.CourseEnrolments
                .FirstAsync(e => e.StudentProfile.Name.Contains("Adam"));
            
            context.AttendanceRecords.AddRange(
                new AttendanceRecord
                {
                    CourseEnrolmentId = billEnrolment.Id,
                    WeekNumber = 1,
                    Date = DateTime.Now.AddDays(-14),
                    Present = true
                },
                new AttendanceRecord
                {
                    CourseEnrolmentId = billEnrolment.Id,
                    WeekNumber = 2,
                    Date = DateTime.Now.AddDays(-7),
                    Present = true
                },
                new AttendanceRecord
                {
                    CourseEnrolmentId = adamEnrolment.Id,
                    WeekNumber = 1,
                    Date = DateTime.Now.AddDays(-14),
                    Present = false
                },
                new AttendanceRecord
                {
                    CourseEnrolmentId = adamEnrolment.Id,
                    WeekNumber = 2,
                    Date = DateTime.Now.AddDays(-7),
                    Present = true
                }
            );
            await context.SaveChangesAsync();
        }
    }
}