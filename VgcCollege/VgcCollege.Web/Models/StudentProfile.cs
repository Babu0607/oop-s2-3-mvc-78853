using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace VgcCollege.Web.Models;

public class StudentProfile
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string StudentNumber { get; set; } = string.Empty;

    [Required]
    public string IdentityUserId { get; set; } = string.Empty;

    [ForeignKey("IdentityUserId")]
    public virtual IdentityUser? User { get; set; }
}