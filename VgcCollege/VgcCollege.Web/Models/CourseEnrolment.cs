using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VgcCollege.Web.Models;

public class CourseEnrolment
{
    [Key]
    public int Id { get; set; }

    // Link to the Student
    [Required]
    public int StudentId { get; set; }
    
    [ForeignKey("StudentId")]
    public virtual StudentProfile? Student { get; set; }

    // Link to the Course
    [Required]
    public int CourseId { get; set; }

    [ForeignKey("CourseId")]
    public virtual Course? Course { get; set; }

    public DateTime EnrolmentDate { get; set; } = DateTime.Now;
}