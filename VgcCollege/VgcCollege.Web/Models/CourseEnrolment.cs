using System.ComponentModel.DataAnnotations;
using VgcCollege.Web.Models;

namespace VgcCollege.Web.Models;

public class CourseEnrolment
{
    public int Id { get; set; }
    
    [Required]
    public int StudentProfileId { get; set; }
    
    [Required]
    public int CourseId { get; set; }
    
    [Required]
    [DataType(DataType.Date)]
    public DateTime EnrolDate { get; set; } = DateTime.Now;
    
    [Required]
    [StringLength(20)]
    public string Status { get; set; } = "Active"; // Active, Completed, Dropped
    
    // Navigation properties
    public StudentProfile? StudentProfile { get; set; }
    public Course? Course { get; set; }
    public ICollection<AttendanceRecord> AttendanceRecords { get; set; } = new List<AttendanceRecord>();
}