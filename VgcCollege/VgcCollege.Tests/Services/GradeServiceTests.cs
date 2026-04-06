using Xunit;
using VgcCollege.Web.Services;

namespace VgcCollege.Tests.Services;

public class GradeServiceTests
{
    private readonly GradeService _gradeService = new GradeService();
    
    [Fact]
    public void CalculatePercentage_ValidScores_ReturnsCorrectPercentage()
    {
        var result = _gradeService.CalculatePercentage(85, 100);
        Assert.Equal(85, result);
    }
    
    [Fact]
    public void CalculatePercentage_ZeroMaxScore_ReturnsZero()
    {
        var result = _gradeService.CalculatePercentage(85, 0);
        Assert.Equal(0, result);
    }
    
    [Fact]
    public void CanStudentViewResult_WhenReleased_ReturnsTrue()
    {
        var result = _gradeService.CanStudentViewResult(true);
        Assert.True(result);
    }
    
    [Fact]
    public void CanStudentViewResult_WhenNotReleased_ReturnsFalse()
    {
        var result = _gradeService.CanStudentViewResult(false);
        Assert.False(result);
    }
}
