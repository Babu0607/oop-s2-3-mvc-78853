using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using VgcCollege.Web.Models;

namespace VgcCollege.Web.Data;
                                     public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) 
    : IdentityDbContext(options)
{
    public DbSet<Branch> Branches { get; set; }
    public DbSet<Course> Courses { get; set; }
    
    public DbSet<StudentProfile> StudentProfiles { get; set; }
    public DbSet<CourseEnrolment> CourseEnrolments { get; set; }
    public DbSet<FacultyProfile> FacultyProfiles { get; set; }
    public DbSet<Assignment> Assignments { get; set; }
    public DbSet<AssignmentResult> AssignmentResults { get; set; }
}