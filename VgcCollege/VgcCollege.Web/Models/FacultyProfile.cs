using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace VgcCollege.Web.Models;

public class FacultyProfile
{
    public int Id { get; set; }
    
    [Required]
    public string IdentityUserId { get; set; } = string.Empty;
    
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;
    
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
    
    [Phone]
    public string Phone { get; set; } = string.Empty;
    
    public IdentityUser? IdentityUser { get; set; }
    public ICollection<FacultyCourse> Courses { get; set; } = new List<FacultyCourse>();
}