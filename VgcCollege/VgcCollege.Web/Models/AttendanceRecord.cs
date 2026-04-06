using System.ComponentModel.DataAnnotations;

namespace VgcCollege.Web.Models;

public class AttendanceRecord
{
    public int Id { get; set; }
    
    [Required]
    public int CourseEnrolmentId { get; set; }
    
    [Required]
    public int WeekNumber { get; set; }
    
    [Required]
    [DataType(DataType.Date)]
    public DateTime Date { get; set; }
    
    [Required]
    public bool Present { get; set; }
    
    // Navigation property
    public CourseEnrolment? CourseEnrolment { get; set; }
}