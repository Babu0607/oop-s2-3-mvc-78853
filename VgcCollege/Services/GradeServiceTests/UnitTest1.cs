using VgcCollege.Web.Services;

namespace GradeServiceTests;

public class GradeServiceTests
{
    private readonly GradeService _gradeService = new();

    [Fact]
    public void CalculatePercentage_ValidScores_ReturnsCorrectPercentage()
    {
        decimal score = 85;
        decimal maxScore = 100;
        
        var result = _gradeService.CalculatePercentage(score, maxScore);
        
        Assert.Equal(85, result);
    }
    
    [Fact]
    public void CalculatePercentage_ZeroMaxScore_ReturnsZero()
    {
        decimal score = 85;
        decimal maxScore = 0;
        
        var result = _gradeService.CalculatePercentage(score, maxScore);
        
        Assert.Equal(0, result);
    }
    
    [Theory]
    [InlineData(95, "A")]
    [InlineData(90, "A")]
    [InlineData(89, "B")]
    [InlineData(80, "B")]
    [InlineData(79, "C")]
    [InlineData(70, "C")]
    [InlineData(69, "D")]
    [InlineData(60, "D")]
    [InlineData(59, "F")]
    [InlineData(50, "F")]
    public void GetGradeLetter_VariousPercentages_ReturnsCorrectGrade(decimal percentage, string expectedGrade)
    {
        var result = _gradeService.GetGradeLetter(percentage);
        
        Assert.Equal(expectedGrade, result);
    }
    
    [Fact]
    public void CanStudentViewResult_ResultsReleased_ReturnsTrue()
    {
        var result = _gradeService.CanStudentViewResult(true);
        
        Assert.True(result);
    }
    
    [Fact]
    public void CanStudentViewResult_ResultsNotReleased_ReturnsFalse()
    {
        var result = _gradeService.CanStudentViewResult(false);
        
        Assert.False(result);
    }
}