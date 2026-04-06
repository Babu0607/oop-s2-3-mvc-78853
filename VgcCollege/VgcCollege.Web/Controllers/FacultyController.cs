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
        var facultyId = await GetCurrentFacultyId();
        if (facultyId == null)
        {
            return RedirectToAction("AccessDenied", "Home");
        }
        
        var myCourses = await _context.FacultyCourses
            .Where(fc => fc.FacultyProfileId == facultyId)
            .Include(fc => fc.Course)
            .ThenInclude(c => c!.Branch)
            .ToListAsync();
        
        ViewBag.CourseCount = myCourses.Count;
        return View(myCourses);
    }
    
    public async Task<IActionResult> StudentsByCourse(int courseId)
    {
        var facultyId = await GetCurrentFacultyId();
        if (facultyId == null || !await IsFacultyAssignedToCourse(facultyId.Value, courseId))
        {
            return RedirectToAction("AccessDenied", "Home");
        }
        
        var enrolments = await _context.CourseEnrolments
            .Include(e => e.StudentProfile)
            .Where(e => e.CourseId == courseId && e.Status == "Active")
            .ToListAsync();
        
        var course = await _context.Courses.FindAsync(courseId);
        ViewBag.Course = course;
        ViewBag.CourseId = courseId;
        
        return View(enrolments);
    }
    
    public async Task<IActionResult> Gradebook(int courseId)
    {
        var facultyId = await GetCurrentFacultyId();
        if (facultyId == null || !await IsFacultyAssignedToCourse(facultyId.Value, courseId))
        {
            return RedirectToAction("AccessDenied", "Home");
        }
        
        var assignments = await _context.Assignments
            .Where(a => a.CourseId == courseId)
            .ToListAsync();
        
        var students = await _context.CourseEnrolments
            .Include(e => e.StudentProfile)
            .Where(e => e.CourseId == courseId && e.Status == "Active")
            .Select(e => e.StudentProfile)
            .ToListAsync();
        
        var results = await _context.AssignmentResults
            .Include(r => r.Assignment)
            .Where(r => assignments.Select(a => a.Id).Contains(r.AssignmentId))
            .ToListAsync();
        
        var course = await _context.Courses.FindAsync(courseId);
        ViewBag.Course = course;
        ViewBag.CourseId = courseId;
        ViewBag.Assignments = assignments;
        ViewBag.Students = students;
        ViewBag.Results = results;
        
        return View();
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SaveAssignmentResult(int assignmentId, int studentId, decimal score, string feedback)
    {
        var assignment = await _context.Assignments.FindAsync(assignmentId);
        if (assignment == null)
        {
            return NotFound();
        }
        
        var facultyId = await GetCurrentFacultyId();
        if (facultyId == null || !await IsFacultyAssignedToCourse(facultyId.Value, assignment.CourseId))
        {
            return RedirectToAction("AccessDenied", "Home");
        }
        
        var result = await _context.AssignmentResults
            .FirstOrDefaultAsync(r => r.AssignmentId == assignmentId && r.StudentProfileId == studentId);
        
        if (result == null)
        {
            result = new AssignmentResult
            {
                AssignmentId = assignmentId,
                StudentProfileId = studentId,
                Score = score,
                Feedback = feedback ?? string.Empty
            };
            _context.AssignmentResults.Add(result);
        }
        else
        {
            result.Score = score;
            result.Feedback = feedback ?? string.Empty;
            _context.Update(result);
        }
        
        await _context.SaveChangesAsync();
        TempData["Success"] = "Result saved successfully!";
        
        return RedirectToAction("Gradebook", new { courseId = assignment.CourseId });
    }
    
    public async Task<IActionResult> StudentContactDetails(int studentId)
    {
        var facultyId = await GetCurrentFacultyId();
        if (facultyId == null)
        {
            return RedirectToAction("AccessDenied", "Home");
        }
        
        var isAuthorized = await _context.CourseEnrolments
            .Where(e => e.StudentProfileId == studentId)
            .Join(_context.FacultyCourses,
                e => e.CourseId,
                fc => fc.CourseId,
                (e, fc) => fc.FacultyProfileId)
            .AnyAsync(fid => fid == facultyId);
        
        if (!isAuthorized)
        {
            return RedirectToAction("AccessDenied", "Home");
        }
        
        var student = await _context.StudentProfiles.FindAsync(studentId);
        if (student == null)
        {
            return NotFound();
        }
        
        return View(student);
    }
    
    public async Task<IActionResult> Attendance(int courseId)
    {
        var facultyId = await GetCurrentFacultyId();
        if (facultyId == null || !await IsFacultyAssignedToCourse(facultyId.Value, courseId))
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
        ViewBag.CourseId = courseId;
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
        
        var facultyId = await GetCurrentFacultyId();
        if (facultyId == null || !await IsFacultyAssignedToCourse(facultyId.Value, enrolment.CourseId))
        {
            return RedirectToAction("AccessDenied", "Home");
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
    private async Task<int?> GetCurrentFacultyId()
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (userId == null) return null;
        
        var faculty = await _context.FacultyProfiles
            .FirstOrDefaultAsync(f => f.IdentityUserId == userId);
        
        return faculty?.Id;
    }
    
    private async Task<bool> IsFacultyAssignedToCourse(int facultyId, int courseId)
    {
        return await _context.FacultyCourses
            .AnyAsync(fc => fc.FacultyProfileId == facultyId && fc.CourseId == courseId);
    }
    
    private int GetCurrentWeekNumber()
    {
        var startDate = new DateTime(2024, 9, 1);
        var daysSinceStart = (DateTime.Now - startDate).Days;
        var weekNumber = (daysSinceStart / 7) + 1;
        return weekNumber > 0 ? weekNumber : 1;
    }
}