using System.ComponentModel.DataAnnotations;

namespace VgcCollege.Web.Models;

public class ExamResult
{
    public int Id { get; set; }
    
    [Required]
    public int ExamId { get; set; }
    
    [Required]
    public int StudentProfileId { get; set; }
    
    [Required]
    [Range(0, 1000)]
    public decimal Score { get; set; }
    
    [StringLength(2)]
    public string Grade { get; set; } = string.Empty;
    
    public Exam? Exam { get; set; }
    public StudentProfile? StudentProfile { get; set; }
}