namespace GradeService;

public class GradeService
{
    public decimal CalculatePercentage(decimal score, decimal maxScore)
    {
        if (maxScore <= 0) return 0;
        return Math.Round((score / maxScore) * 100, 2);
    }
    
    public string GetGradeLetter(decimal percentage)
    {
        if (percentage >= 90) return "A";
        if (percentage >= 80) return "B";
        if (percentage >= 70) return "C";
        if (percentage >= 60) return "D";
        return "F";
    }
    
    public bool CanStudentViewResult(bool resultsReleased)
    {
        return resultsReleased;
    }
}