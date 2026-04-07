using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VgcCollege.Web.Data;
using VgcCollege.Web.Models;

namespace VgcCollege.Web.Controllers;

[Authorize(Roles = "Faculty,Admin")]
public class FacultyController : Controller
{
    private readonly ApplicationDbContext _context;
    
    public FacultyController(ApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<IActionResult> Index()
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        Console.WriteLine($"=== Dashboard ===");
        Console.WriteLine($"UserId: {userId}");
        
        var faculty = await _context.FacultyProfiles
            .FirstOrDefaultAsync(f => f.IdentityUserId == userId);
        
        if (faculty == null)
        {
            Console.WriteLine("Faculty not found!");
            return RedirectToAction("AccessDenied", "Home");
        }
        
        Console.WriteLine($"Faculty found: {faculty.Name} (ID: {faculty.Id})");
        
        var myCourses = await _context.FacultyCourses
            .Where(fc => fc.FacultyProfileId == faculty.Id)
            .Include(fc => fc.Course)
            .ThenInclude(c => c.Branch)
            .ToListAsync();
        
        Console.WriteLine($"Courses found: {myCourses.Count}");
        
        return View(myCourses);
    }
    public async Task<IActionResult> Gradebook(int courseId)
    {
        Console.WriteLine($"=== Gradebook Debug ===");
        Console.WriteLine($"CourseId received: {courseId}");
        
        if (!User.Identity.IsAuthenticated)
        {
            Console.WriteLine("ERROR: User not authenticated");
            return RedirectToAction("AccessDenied", "Home");
        }
        
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        Console.WriteLine($"UserId: {userId}");
        
        var faculty = await _context.FacultyProfiles
            .FirstOrDefaultAsync(f => f.IdentityUserId == userId);
        
        if (faculty == null)
        {
            Console.WriteLine("ERROR: No faculty profile found");
            return RedirectToAction("AccessDenied", "Home");
        }
        
        Console.WriteLine($"Faculty found: {faculty.Name} (ID: {faculty.Id})");
        
        var isAssigned = await _context.FacultyCourses
            .AnyAsync(fc => fc.FacultyProfileId == faculty.Id && fc.CourseId == courseId);
        
        Console.WriteLine($"Is assigned to course {courseId}: {isAssigned}");
        
        if (!isAssigned)
        {
            Console.WriteLine("ERROR: Faculty not assigned to this course");
            return RedirectToAction("AccessDenied", "Home");
        }
        
        ViewBag.CourseId = courseId;
        ViewBag.FacultyName = faculty.Name;
        
        return View();
    }
    public async Task<IActionResult> StudentsByCourse(int courseId)
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        var faculty = await _context.FacultyProfiles
            .FirstOrDefaultAsync(f => f.IdentityUserId == userId);
        
        if (faculty == null)
        {
            return RedirectToAction("AccessDenied", "Home");
        }
        
        var isAssigned = await _context.FacultyCourses
            .AnyAsync(fc => fc.FacultyProfileId == faculty.Id && fc.CourseId == courseId);
        
        if (!isAssigned)
        {
            return RedirectToAction("AccessDenied", "Home");
        }
        
        var enrolments = await _context.CourseEnrolments
            .Include(e => e.StudentProfile)
            .Where(e => e.CourseId == courseId && e.Status == "Active")
            .ToListAsync();
        
        var course = await _context.Courses.FindAsync(courseId);
        ViewBag.Course = course;
        
        return View(enrolments);
    }
    public async Task<IActionResult> Attendance(int courseId)
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        var faculty = await _context.FacultyProfiles
            .FirstOrDefaultAsync(f => f.IdentityUserId == userId);
        
        if (faculty == null)
        {
            return RedirectToAction("AccessDenied", "Home");
        }
        
        var isAssigned = await _context.FacultyCourses
            .AnyAsync(fc => fc.FacultyProfileId == faculty.Id && fc.CourseId == courseId);
        
        if (!isAssigned)
        {
            return RedirectToAction("AccessDenied", "Home");
        }
        
        var enrolments = await _context.CourseEnrolments
            .Include(e => e.StudentProfile)
            .Include(e => e.AttendanceRecords)
            .Where(e => e.CourseId == courseId && e.Status == "Active")
            .ToListAsync();
        
        var course = await _context.Courses.FindAsync(courseId);
        ViewBag.Course = course;
        ViewBag.CurrentWeek = GetCurrentWeekNumber();
        
        return View(enrolments);
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> MarkAttendance(int enrolmentId, int weekNumber, DateTime date, bool present)
    {
        var enrolment = await _context.CourseEnrolments
            .Include(e => e.Course)
            .FirstOrDefaultAsync(e => e.Id == enrolmentId);
        
        if (enrolment == null)
        {
            return NotFound();
        }
        
        var attendance = await _context.AttendanceRecords
            .FirstOrDefaultAsync(a => a.CourseEnrolmentId == enrolmentId && a.WeekNumber == weekNumber);
        
        if (attendance == null)
        {
            attendance = new AttendanceRecord
            {
                CourseEnrolmentId = enrolmentId,
                WeekNumber = weekNumber,
                Date = date,
                Present = present
            };
            _context.AttendanceRecords.Add(attendance);
        }
        else
        {
            attendance.Present = present;
            _context.Update(attendance);
        }
        
        await _context.SaveChangesAsync();
        TempData["Success"] = $"Attendance marked for week {weekNumber}!";
        
        return RedirectToAction("Attendance", new { courseId = enrolment.CourseId });
    }
    public async Task<IActionResult> StudentContactDetails(int studentId)
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        var faculty = await _context.FacultyProfiles
            .FirstOrDefaultAsync(f => f.IdentityUserId == userId);
        
        if (faculty == null)
        {
            return RedirectToAction("AccessDenied", "Home");
        }
        
        var isAuthorized = await _context.CourseEnrolments
            .Where(e => e.StudentProfileId == studentId)
            .Join(_context.FacultyCourses,
                e => e.CourseId,
                fc => fc.CourseId,
                (e, fc) => fc.FacultyProfileId)
            .AnyAsync(fid => fid == faculty.Id);
        
        if (!isAuthorized)
        {
            return RedirectToAction("AccessDenied", "Home");
        }
        
        var student = await _context.StudentProfiles.FindAsync(studentId);
        return View(student);
    }
    private int GetCurrentWeekNumber()
    {
        var startDate = new DateTime(2024, 9, 1);
        var daysSinceStart = (DateTime.Now - startDate).Days;
        var weekNumber = (daysSinceStart / 7) + 1;
        return weekNumber > 0 ? weekNumber : 1;
    }
}