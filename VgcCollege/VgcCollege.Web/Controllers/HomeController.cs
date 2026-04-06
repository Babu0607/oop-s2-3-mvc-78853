using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace VgcCollege.Web.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
    
    [Authorize]
    public IActionResult Dashboard()
    {
        if (User.IsInRole("Admin"))
        {
            return RedirectToAction("Index", "Admin");
        }
        if (User.IsInRole("Faculty"))
        {
            return RedirectToAction("Index", "Faculty");
        }
        if (User.IsInRole("Student"))
        {
            return RedirectToAction("Index", "Student");
        }
        
        return RedirectToAction("Index");
    }
    
    public IActionResult AccessDenied()
    {
        return View();
    }
    
    public IActionResult Error()
    {
        return View();
    }
}