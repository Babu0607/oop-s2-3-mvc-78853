using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VgcCollege.Web.Data;
using VgcCollege.Web.Models;

namespace VgcCollege.Web.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly ApplicationDbContext _context;
    
    public AdminController(ApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<IActionResult> Index()
    {
        var stats = new
        {
            BranchCount = await _context.Branches.CountAsync(),
            CourseCount = await _context.Courses.CountAsync(),
            StudentCount = await _context.StudentProfiles.CountAsync(),
            FacultyCount = await _context.FacultyProfiles.CountAsync()
        };
        
        ViewBag.Stats = stats;
        return View();
    }
    public async Task<IActionResult> Branches()
    {
        var branches = await _context.Branches.ToListAsync();
        return View(branches);
    }
    
    [HttpGet]
    public IActionResult CreateBranch()
    {
        return View();
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateBranch(Branch branch)
    {
        Console.WriteLine($"=== Creating Branch ===");
        Console.WriteLine($"Name: {branch.Name}");
        Console.WriteLine($"Address: {branch.Address}");
        
        if (ModelState.IsValid)
        {
            try
            {
                _context.Branches.Add(branch);
                await _context.SaveChangesAsync();
                Console.WriteLine($"SUCCESS! Branch saved with ID: {branch.Id}");
                TempData["Success"] = "Branch created successfully!";
                return RedirectToAction(nameof(Branches));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR: {ex.Message}");
                TempData["Error"] = $"Error: {ex.Message}";
                return View(branch);
            }
        }
        
        var errors = ModelState.Values.SelectMany(v => v.Errors);
        foreach (var error in errors)
        {
            Console.WriteLine($"Validation Error: {error.ErrorMessage}");
        }
        
        return View(branch);
    }
    
    [HttpGet]
    public async Task<IActionResult> EditBranch(int id)
    {
        var branch = await _context.Branches.FindAsync(id);
        if (branch == null)
        {
            return NotFound();
        }
        return View(branch);
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditBranch(int id, Branch branch)
    {
        if (id != branch.Id)
        {
            return NotFound();
        }
        
        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(branch);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Branch updated successfully!";
                return RedirectToAction(nameof(Branches));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Branches.Any(e => e.Id == branch.Id))
                {
                    return NotFound();
                }
                throw;
            }
        }
        return View(branch);
    }
    public async Task<IActionResult> Courses()
    {
        var courses = await _context.Courses
            .Include(c => c.Branch)
            .ToListAsync();
        return View(courses);
    }
    
    [HttpGet]
    public async Task<IActionResult> CreateCourse()
    {
        ViewBag.Branches = new SelectList(await _context.Branches.ToListAsync(), "Id", "Name");
        return View();
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateCourse(Course course)
    {
        Console.WriteLine($"=== Creating Course ===");
        Console.WriteLine($"Name: {course.Name}");
        Console.WriteLine($"BranchId: {course.BranchId}");
        
        if (ModelState.IsValid)
        {
            try
            {
                _context.Courses.Add(course);
                await _context.SaveChangesAsync();
                Console.WriteLine($"SUCCESS! Course saved with ID: {course.Id}");
                TempData["Success"] = "Course created successfully!";
                return RedirectToAction(nameof(Courses));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR: {ex.Message}");
                TempData["Error"] = $"Error: {ex.Message}";
                ViewBag.Branches = new SelectList(await _context.Branches.ToListAsync(), "Id", "Name");
                return View(course);
            }
        }
        
        var errors = ModelState.Values.SelectMany(v => v.Errors);
        foreach (var error in errors)
        {
            Console.WriteLine($"Validation Error: {error.ErrorMessage}");
        }
        
        ViewBag.Branches = new SelectList(await _context.Branches.ToListAsync(), "Id", "Name");
        return View(course);
    }
    
    [HttpGet]
    public async Task<IActionResult> EditCourse(int id)
    {
        var course = await _context.Courses.FindAsync(id);
        if (course == null)
        {
            return NotFound();
        }
        ViewBag.Branches = new SelectList(await _context.Branches.ToListAsync(), "Id", "Name", course.BranchId);
        return View(course);
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditCourse(int id, Course course)
    {
        if (id != course.Id)
        {
            return NotFound();
        }
        
        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(course);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Course updated successfully!";
                return RedirectToAction(nameof(Courses));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Courses.Any(e => e.Id == course.Id))
                {
                    return NotFound();
                }
                throw;
            }
        }
        ViewBag.Branches = new SelectList(await _context.Branches.ToListAsync(), "Id", "Name", course.BranchId);
        return View(course);
    }
    public async Task<IActionResult> Enrolments()
    {
        var enrolments = await _context.CourseEnrolments
            .Include(e => e.StudentProfile)
            .Include(e => e.Course)
            .ThenInclude(c => c!.Branch)
            .ToListAsync();
        return View(enrolments);
    }
    
    [HttpGet]
    public async Task<IActionResult> CreateEnrolment()
    {
        ViewBag.Students = new SelectList(await _context.StudentProfiles.ToListAsync(), "Id", "Name");
        ViewBag.Courses = new SelectList(await _context.Courses.Include(c => c.Branch).ToListAsync(), "Id", "Name");
        return View();
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateEnrolment(CourseEnrolment enrolment)
    {
        if (ModelState.IsValid)
        {
            _context.CourseEnrolments.Add(enrolment);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Enrolment created successfully!";
            return RedirectToAction(nameof(Enrolments));
        }
        
        ViewBag.Students = new SelectList(await _context.StudentProfiles.ToListAsync(), "Id", "Name");
        ViewBag.Courses = new SelectList(await _context.Courses.Include(c => c.Branch).ToListAsync(), "Id", "Name");
        return View(enrolment);
    }
    
    [HttpGet]
    public async Task<IActionResult> EditEnrolment(int id)
    {
        var enrolment = await _context.CourseEnrolments
            .Include(e => e.StudentProfile)
            .Include(e => e.Course)
            .FirstOrDefaultAsync(e => e.Id == id);
            
        if (enrolment == null)
        {
            return NotFound();
        }
        
        ViewBag.Students = new SelectList(await _context.StudentProfiles.ToListAsync(), "Id", "Name", enrolment.StudentProfileId);
        ViewBag.Courses = new SelectList(await _context.Courses.Include(c => c.Branch).ToListAsync(), "Id", "Name", enrolment.CourseId);
        return View(enrolment);
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditEnrolment(int id, CourseEnrolment enrolment)
    {
        if (id != enrolment.Id)
        {
            return NotFound();
        }
        
        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(enrolment);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Enrolment updated successfully!";
                return RedirectToAction(nameof(Enrolments));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.CourseEnrolments.Any(e => e.Id == enrolment.Id))
                {
                    return NotFound();
                }
                throw;
            }
        }
        
        ViewBag.Students = new SelectList(await _context.StudentProfiles.ToListAsync(), "Id", "Name", enrolment.StudentProfileId);
        ViewBag.Courses = new SelectList(await _context.Courses.Include(c => c.Branch).ToListAsync(), "Id", "Name", enrolment.CourseId);
        return View(enrolment);
    }
    public async Task<IActionResult> ManageExams()
    {
        var exams = await _context.Exams
            .Include(e => e.Course)
            .ToListAsync();
        return View(exams);
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleResultRelease(int examId)
    {
        var exam = await _context.Exams.FindAsync(examId);
        if (exam != null)
        {
            exam.ResultsReleased = !exam.ResultsReleased;
            await _context.SaveChangesAsync();
            TempData["Success"] = $"Results {(exam.ResultsReleased ? "released" : "made provisional")} successfully!";
        }
        return RedirectToAction(nameof(ManageExams));
    }
    public async Task<IActionResult> Students()
    {
        var students = await _context.StudentProfiles
            .Include(s => s.IdentityUser)
            .ToListAsync();
        return View(students);
    }
    
    [HttpGet]
    public IActionResult CreateStudent()
    {
        return View();
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateStudent(StudentProfile student)
    {
        if (ModelState.IsValid)
        {
            _context.StudentProfiles.Add(student);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Student created successfully!";
            return RedirectToAction(nameof(Students));
        }
        return View(student);
    }
    
    public async Task<IActionResult> Faculty()
    {
        var faculty = await _context.FacultyProfiles
            .Include(f => f.IdentityUser)
            .ToListAsync();
        return View(faculty);
    }
    
    [HttpGet]
    public IActionResult CreateFaculty()
    {
        return View();
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateFaculty(FacultyProfile faculty)
    {
        if (ModelState.IsValid)
        {
            _context.FacultyProfiles.Add(faculty);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Faculty created successfully!";
            return RedirectToAction(nameof(Faculty));
        }
        return View(faculty);
    }
}