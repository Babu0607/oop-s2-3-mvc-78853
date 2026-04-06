using System.ComponentModel.DataAnnotations;
using System.Linq;
using VgcCollege.Web.Models;
using Xunit;

namespace VgcCollege.Tests.Models;

public class ModelValidationTests
{
    private List<ValidationResult> ValidateModel(object model)
    {
        var context = new ValidationContext(model);
        var results = new List<ValidationResult>();
        Validator.TryValidateObject(model, context, results, true);
        return results;
    }
    
    [Fact]
    public void Branch_WithoutName_HasValidationError()
    {
        var branch = new Branch { Address = "123 Main St" };
        var results = ValidateModel(branch);
        Assert.Contains(results, r => r.MemberNames.Contains("Name"));
    }
    
    [Fact]
    public void Branch_WithoutAddress_HasValidationError()
    {
        var branch = new Branch { Name = "Dublin Campus" };
        var results = ValidateModel(branch);
        Assert.Contains(results, r => r.MemberNames.Contains("Address"));
    }
    
    [Fact]
    public void Course_WithoutName_HasValidationError()
    {
        var course = new Course { BranchId = 1, StartDate = DateTime.Now, EndDate = DateTime.Now.AddMonths(6) };
        var results = ValidateModel(course);
        Assert.Contains(results, r => r.MemberNames.Contains("Name"));
    }
}
