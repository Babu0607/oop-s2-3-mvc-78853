using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace VgcCollege.Web.Controllers;

public class AccountController : Controller
{
    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        return Redirect($"/Identity/Account/Login?returnUrl={returnUrl}");
    }
    
    [HttpGet]
    public IActionResult Register()
    {
        return Redirect("/Identity/Account/Register");
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        return Redirect("/Identity/Account/Logout");
    }
    
    [HttpGet]
    public IActionResult AccessDenied()
    {
        return View();
    }
}