using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using VgcCollege.Web.Data;
using VgcCollege.Web.Models;

namespace VgcCollege.Web.Services;

public static class SeedData
{
    public static async Task Initialize(IServiceProvider serviceProvider)
    {
        using var context = new ApplicationDbContext(
            serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>());

        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();

        // 1. Roles: Added 'Faculty' as per brief
        string[] roleNames = { "Admin", "Faculty", "Student" };
        foreach (var roleName in roleNames)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }

        // 2. Seed Admin
        var adminEmail = "adm@vgc.ie";
        if (await userManager.FindByEmailAsync(adminEmail) == null)
        {
            var user = new IdentityUser { UserName = adminEmail, Email = adminEmail, EmailConfirmed = true };
            await userManager.CreateAsync(user, "Admin123!");
            await userManager.AddToRoleAsync(user, "Admin");
        }

        // 3. Seed Faculty (New requirement)
        var facultyEmail = "teacher@vgc.ie";
        if (await userManager.FindByEmailAsync(facultyEmail) == null)
        {
            var user = new IdentityUser { UserName = facultyEmail, Email = facultyEmail, EmailConfirmed = true };
            await userManager.CreateAsync(user, "Teacher123!");
            await userManager.AddToRoleAsync(user, "Faculty");
            
            // Create the Faculty Profile link
            if (!context.FacultyProfiles.Any(f => f.Email == facultyEmail))
            {
                context.FacultyProfiles.Add(new FacultyProfile { 
                    IdentityUserId = user.Id, 
                    Name = "Professor John Doe", 
                    Email = facultyEmail 
                });
            }
        }

        // 4. Seed Student
        var studentEmail = "stdnt@vgc.ie";
        if (await userManager.FindByEmailAsync(studentEmail) == null)
        {
            var user = new IdentityUser { UserName = studentEmail, Email = studentEmail, EmailConfirmed = true };
            await userManager.CreateAsync(user, "Student123!");
            await userManager.AddToRoleAsync(user, "Student");

            // Create the Student Profile link
            if (!context.StudentProfiles.Any(s => s.Email == studentEmail))
            {
                context.StudentProfiles.Add(new StudentProfile { 
                    IdentityUserId = user.Id, 
                    Name = "Alice Student", 
                    Email = studentEmail,
                    StudentNumber = "VGC-2026-001"
                });
            }
        }

        // 5. Seed 3 Branches (Required: "three branches in Ireland")
        if (context.Branches.Count() < 3)
        {
            // Clear existing if necessary or just add missing
            if (!context.Branches.Any(b => b.Name == "Limerick City"))
                context.Branches.Add(new Branch { Name = "Limerick City", Address = "20 City Centre, Limerick" });
            
            if (!context.Branches.Any(b => b.Name == "Cork Campus"))
                context.Branches.Add(new Branch { Name = "Cork Campus", Address = "30 Main Street, Cork" });
            
            if (!context.Branches.Any(b => b.Name == "Dublin HQ"))
                context.Branches.Add(new Branch { Name = "Dublin HQ", Address = "Amesbury St, Dublin 1" });
            
            await context.SaveChangesAsync();
        }

        // 6. Seed Courses
        if (!context.Courses.Any())
        {
            var branch = await context.Branches.FirstOrDefaultAsync();
            if (branch != null)
            {
                context.Courses.Add(new Course 
                { 
                    Name = "Marketing Management", 
                    BranchId = branch.Id,
                    StartDate = DateTime.Now.AddDays(14),
                    EndDate = DateTime.Now.AddYears(1)
                });
                await context.SaveChangesAsync();
            }
        }
    }
}