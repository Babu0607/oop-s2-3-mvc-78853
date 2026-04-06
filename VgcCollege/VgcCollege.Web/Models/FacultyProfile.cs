using System.ComponentModel.DataAnnotations;
namespace VgcCollege.Web.Models;

public class FacultyProfile
{
    [Key]
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string IdentityUserId { get; set; } = string.Empty;
    
    // Which course does this faculty member teach? (Simplified)
    public int? AssignedCourseId { get; set; }
    public virtual Course? AssignedCourse { get; set; }
}