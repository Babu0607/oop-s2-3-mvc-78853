namespace VgcCollege.Web.Models;

public class Assignment
{
    
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public int MaxScore { get; set; } = 100;
    public int CourseId { get; set; }
    public virtual Course? Course { get; set; }
    
}