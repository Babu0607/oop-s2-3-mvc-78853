using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VgcCollege.Web.Data;
using VgcCollege.Web.Models;

namespace VgcCollege.Web.Controllers;

[Authorize(Roles = "Student")]
public class StudentController : Controller
{
    private readonly ApplicationDbContext _context;
    
    public StudentController(ApplicationDbContext context)
    {
        _context = context;
    }
    
    public async Task<IActionResult> Index()
    {
        var studentId = await GetCurrentStudentId();
        if (studentId == null)
        {
            return RedirectToAction("AccessDenied", "Home");
        }
        
        var student = await _context.StudentProfiles.FindAsync(studentId);
        var enrolments = await _context.CourseEnrolments
            .Include(e => e.Course)
            .ThenInclude(c => c.Branch)
            .Where(e => e.StudentProfileId == studentId && e.Status == "Active")
            .ToListAsync();
        
        ViewBag.Student = student;
        ViewBag.Enrolments = enrolments;
        
        return View();
    }
    
    public async Task<IActionResult> MyCourses()
    {
        var studentId = await GetCurrentStudentId();
        if (studentId == null)
        {
            return RedirectToAction("AccessDenied", "Home");
        }
        
        var enrolments = await _context.CourseEnrolments
            .Include(e => e.Course)
            .ThenInclude(c => c.Branch)
            .Where(e => e.StudentProfileId == studentId)
            .ToListAsync();
        
        return View(enrolments);
    }
    
    public async Task<IActionResult> MyGrades()
    {
        var studentId = await GetCurrentStudentId();
        if (studentId == null)
        {
            return RedirectToAction("AccessDenied", "Home");
        }
        
        var assignmentResults = await _context.AssignmentResults
            .Include(r => r.Assignment)
            .ThenInclude(a => a.Course)
            .Where(r => r.StudentProfileId == studentId)
            .ToListAsync();
        
        var examResults = await _context.ExamResults
            .Include(r => r.Exam)
            .ThenInclude(e => e.Course)
            .Where(r => r.StudentProfileId == studentId && r.Exam.ResultsReleased == true)
            .ToListAsync();
        
        ViewBag.AssignmentResults = assignmentResults;
        ViewBag.ExamResults = examResults;
        
        return View();
    }
    public async Task<IActionResult> MyAttendance()
    {
        var studentId = await GetCurrentStudentId();
        if (studentId == null)
        {
            return RedirectToAction("AccessDenied", "Home");
        }
        
        var attendanceRecords = await _context.AttendanceRecords
            .Include(a => a.CourseEnrolment)
            .ThenInclude(e => e.Course)
            .Where(a => a.CourseEnrolment.StudentProfileId == studentId)
            .OrderBy(a => a.Date)
            .ToListAsync();
        
        return View(attendanceRecords);
    }
    public async Task<IActionResult> CourseDetails(int courseId)
    {
        var studentId = await GetCurrentStudentId();
        if (studentId == null)
        {
            return RedirectToAction("AccessDenied", "Home");
        }
        
        var isEnrolled = await _context.CourseEnrolments
            .AnyAsync(e => e.StudentProfileId == studentId && e.CourseId == courseId);
        
        if (!isEnrolled)
        {
            return RedirectToAction("AccessDenied", "Home");
        }
        
        var course = await _context.Courses
            .Include(c => c.Branch)
            .FirstOrDefaultAsync(c => c.Id == courseId);
        
        var assignments = await _context.Assignments
            .Where(a => a.CourseId == courseId)
            .ToListAsync();
        
        var exams = await _context.Exams
            .Where(e => e.CourseId == courseId)
            .ToListAsync();
        
        var assignmentResults = await _context.AssignmentResults
            .Where(r => r.StudentProfileId == studentId && assignments.Select(a => a.Id).Contains(r.AssignmentId))
            .ToListAsync();
        
        var examResults = await _context.ExamResults
            .Where(r => r.StudentProfileId == studentId && exams.Select(e => e.Id).Contains(r.ExamId) && r.Exam.ResultsReleased == true)
            .ToListAsync();
        
        ViewBag.Course = course;
        ViewBag.Assignments = assignments;
        ViewBag.Exams = exams;
        ViewBag.AssignmentResults = assignmentResults;
        ViewBag.ExamResults = examResults;
        
        return View();
    }
    [HttpGet]
    public IActionResult Test()
    {
        return Content("Student Controller is working! Available URLs: /Student, /Student/MyCourses, /Student/MyGrades, /Student/MyAttendance");
    }
    
    private async Task<int?> GetCurrentStudentId()
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (userId == null) return null;
        
        var student = await _context.StudentProfiles
            .FirstOrDefaultAsync(s => s.IdentityUserId == userId);
        
        return student?.Id;
    }
}