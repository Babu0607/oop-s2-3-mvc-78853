using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using VgcCollege.Web.Models;

namespace VgcCollege.Web.Models;

public class StudentProfile
{
    public int Id { get; set; }
    
    [Required]
    public string IdentityUserId { get; set; } = string.Empty;
    
    [Required]
    [StringLength(50)]
    public string StudentNumber { get; set; } = string.Empty;
    
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;
    
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
    
    [Phone]
    public string Phone { get; set; } = string.Empty;
    
    [StringLength(200)]
    public string Address { get; set; } = string.Empty;
    
    [DataType(DataType.Date)]
    public DateTime DateOfBirth { get; set; }
    
    public IdentityUser? IdentityUser { get; set; }
    public ICollection<CourseEnrolment> Enrolments { get; set; } = new List<CourseEnrolment>();
    public ICollection<AssignmentResult> AssignmentResults { get; set; } = new List<AssignmentResult>();
    public ICollection<ExamResult> ExamResults { get; set; } = new List<ExamResult>();
    public ICollection<AttendanceRecord> AttendanceRecords { get; set; } = new List<AttendanceRecord>();
}