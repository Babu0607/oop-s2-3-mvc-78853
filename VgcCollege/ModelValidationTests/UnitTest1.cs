using System.ComponentModel.DataAnnotations;
using VgcCollege.Web.Models;

namespace ModelValidationTests;

public class ModelValidationTests
{
    [Fact]
    public void Branch_MissingName_HasValidationError()
    {
        var branch = new Branch { Address = "Test Address" };
        var validationResults = ValidateModel(branch);
        Assert.Contains(validationResults, v => v.MemberNames.Contains("Name"));
    }
    
    [Fact]
    public void Branch_MissingAddress_HasValidationError()
    {
        var branch = new Branch { Name = "Test Branch" };
        var validationResults = ValidateModel(branch);
        Assert.Contains(validationResults, v => v.MemberNames.Contains("Address"));
    }
    
    [Fact]
    public void Course_MissingName_HasValidationError()
    {
        var course = new Course { BranchId = 1, StartDate = DateTime.Now, EndDate = DateTime.Now.AddMonths(6) };
        var validationResults = ValidateModel(course);
        Assert.Contains(validationResults, v => v.MemberNames.Contains("Name"));
    }
    
    [Fact]
    public void Assignment_MaxScoreZero_HasValidationError()
    {
        var assignment = new Assignment { CourseId = 1, Title = "Test", MaxScore = 0, DueDate = DateTime.Now.AddDays(7) };
        var validationResults = ValidateModel(assignment);
        Assert.Contains(validationResults, v => v.MemberNames.Contains("MaxScore"));
    }
    
    [Fact]
    public void StudentProfile_MissingStudentNumber_HasValidationError()
    {
        var student = new StudentProfile 
        { 
            Name = "Test Student", 
            Email = "test@test.com",
            IdentityUserId = "user123"
        };
        var validationResults = ValidateModel(student);
        Assert.Contains(validationResults, v => v.MemberNames.Contains("StudentNumber"));
    }
    private List<ValidationResult> ValidateModel(object model)
    {
        var validationContext = new ValidationContext(model);
        var validationResults = new List<ValidationResult>();
        Validator.TryValidateObject(model, validationContext, validationResults, true);
        return validationResults;
    }
}