using System.ComponentModel.DataAnnotations;
using VgcCollege.Web.Models;

namespace VgcCollege.Web.Models;

public class Course
{
    public int Id { get; set; }
    
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;
    
    [Required]
    public int BranchId { get; set; }
    
    [Required]
    [DataType(DataType.Date)]
    public DateTime StartDate { get; set; }
    
    [Required]
    [DataType(DataType.Date)]
    public DateTime EndDate { get; set; }
    
    public Branch? Branch { get; set; }
    public ICollection<CourseEnrolment> Enrolments { get; set; } = new List<CourseEnrolment>();
    public ICollection<Assignment> Assignments { get; set; } = new List<Assignment>();
    public ICollection<Exam> Exams { get; set; } = new List<Exam>();
    public ICollection<FacultyCourse> FacultyCourses { get; set; } = new List<FacultyCourse>();
}