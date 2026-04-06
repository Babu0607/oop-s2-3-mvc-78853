using VgcCollege.Web.Models;

namespace VgcCollege.Web.Models;

public class Course
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int BranchId { get; set; }
    public virtual Branch? Branch { get; set; }
    
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    public bool ResultsReleased { get; set; } = false; 
}