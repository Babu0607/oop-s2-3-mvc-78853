namespace EnrolmentServiceTests;

public class EnrolmentServiceTests
{
    [Fact]
    public void CanEnrol_StudentAlreadyEnrolled_ReturnsFalse()
    {
        bool isAlreadyEnrolled = true;
        bool canEnrol = !isAlreadyEnrolled;
        
        Assert.False(canEnrol);
    }
    
    [Fact]
    public void CanEnrol_StudentNotEnrolled_ReturnsTrue()
    {
        bool isAlreadyEnrolled = false;
        bool canEnrol = !isAlreadyEnrolled;
        
        Assert.True(canEnrol);
    }
}