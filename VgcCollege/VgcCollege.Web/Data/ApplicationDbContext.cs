using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using VgcCollege.Web.Models;

namespace VgcCollege.Web.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext(options)
{
    public DbSet<Branch> Branches { get; set; }
    public DbSet<Course> Courses { get; set; }
    public DbSet<StudentProfile> StudentProfiles { get; set; }
    public DbSet<FacultyProfile> FacultyProfiles { get; set; }
    public DbSet<CourseEnrolment> CourseEnrolments { get; set; }
    public DbSet<AttendanceRecord> AttendanceRecords { get; set; }
    public DbSet<Assignment> Assignments { get; set; }
    public DbSet<AssignmentResult> AssignmentResults { get; set; }
    public DbSet<Exam> Exams { get; set; }
    public DbSet<ExamResult> ExamResults { get; set; }
    public DbSet<FacultyCourse> FacultyCourses { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<FacultyCourse>()
            .HasOne(fc => fc.FacultyProfile)
            .WithMany(fp => fp.Courses)
            .HasForeignKey(fc => fc.FacultyProfileId);
            
        modelBuilder.Entity<FacultyCourse>()
            .HasOne(fc => fc.Course)
            .WithMany(c => c.FacultyCourses)
            .HasForeignKey(fc => fc.CourseId);
            
        modelBuilder.Entity<StudentProfile>()
            .HasIndex(s => s.StudentNumber)
            .IsUnique();
    }
}