using System.ComponentModel.DataAnnotations;

namespace VgcCollege.Web.Models;

public class FacultyCourse
{
    public int Id { get; set; }
    
    [Required]
    public int FacultyProfileId { get; set; }
    
    [Required]
    public int CourseId { get; set; }
    
    public FacultyProfile? FacultyProfile { get; set; }
    public Course? Course { get; set; }
}