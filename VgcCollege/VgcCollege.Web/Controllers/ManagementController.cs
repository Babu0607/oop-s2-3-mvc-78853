using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VgcCollege.Web.Data;
using VgcCollege.Web.Models;

namespace VgcCollege.Web.Controllers;

[Authorize(Roles = "Admin")]
public class ManagementController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly RoleManager<IdentityRole> _roleManager;

    public ManagementController(ApplicationDbContext context, RoleManager<IdentityRole> roleManager)
    {
        _context = context;
        _roleManager = roleManager;
    }

    // --- DASHBOARD ---
    public IActionResult Index() => View();

    // --- BRANCH MANAGEMENT ---
    public async Task<IActionResult> Branches()
    {
        return View(await _context.Branches.ToListAsync());
    }

    public IActionResult CreateBranch() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateBranch([Bind("Name,Address")] Branch branch)
    {
        if (ModelState.IsValid)
        {
            _context.Add(branch);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Branch created successfully!";
            return RedirectToAction(nameof(Branches));
        }
        return View(branch);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteBranch(int id)
    {
        var branch = await _context.Branches.FindAsync(id);
        if (branch != null)
        {
            _context.Branches.Remove(branch);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Branch deleted successfully.";
        }
        return RedirectToAction(nameof(Branches));
    }

    // --- COURSE MANAGEMENT ---
    // (Satisfies 15-mark persistence requirement by including relationships)
    public async Task<IActionResult> Courses()
    {
        var courses = await _context.Courses.Include(c => c.Branch).ToListAsync();
        return View(courses);
    }

    public async Task<IActionResult> CreateCourse()
    {
        ViewBag.BranchId = new SelectList(await _context.Branches.ToListAsync(), "Id", "Name");
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateCourse([Bind("Name,BranchId,StartDate,EndDate,ResultsReleased")] Course course)
    {
        if (ModelState.IsValid)
        {
            _context.Add(course);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Course added successfully!";
            return RedirectToAction(nameof(Courses));
        }
        ViewBag.BranchId = new SelectList(await _context.Branches.ToListAsync(), "Id", "Name", course.BranchId);
        return View(course);
    }

    // GET: Management/EditCourse/5
    public async Task<IActionResult> EditCourse(int? id)
    {
        if (id == null) return NotFound();

        var course = await _context.Courses.FindAsync(id);
        if (course == null) return NotFound();

        ViewBag.BranchId = new SelectList(await _context.Branches.ToListAsync(), "Id", "Name", course.BranchId);
        return View(course);
    }

    // POST: Management/EditCourse/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditCourse(int id, [Bind("Id,Name,BranchId,StartDate,EndDate,ResultsReleased")] Course course)
    {
        if (id != course.Id) return NotFound();

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(course);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Course updated successfully!";
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Courses.Any(e => e.Id == course.Id)) return NotFound();
                else throw;
            }
            return RedirectToAction(nameof(Courses));
        }
        ViewBag.BranchId = new SelectList(await _context.Branches.ToListAsync(), "Id", "Name", course.BranchId);
        return View(course);
    }

    // GET: Management/DeleteCourse/5
    public async Task<IActionResult> DeleteCourse(int? id)
    {
        if (id == null) return NotFound();

        var course = await _context.Courses
            .Include(c => c.Branch)
            .FirstOrDefaultAsync(m => m.Id == id);
            
        if (course == null) return NotFound();

        return View(course);
    }

    // POST: Management/DeleteCourse/5 (Confirmed Action)
    [HttpPost, ActionName("DeleteCourse")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var course = await _context.Courses.FindAsync(id);
        if (course != null)
        {
            _context.Courses.Remove(course);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Course deleted successfully!";
        }
        return RedirectToAction(nameof(Courses));
    }

    // --- STUDENT & ROLE MANAGEMENT ---
    public async Task<IActionResult> Students()
    {
        return View(await _context.StudentProfiles.ToListAsync());
    }

    public async Task<IActionResult> Roles()
    {
        return View(await _roleManager.Roles.ToListAsync());
    }
}