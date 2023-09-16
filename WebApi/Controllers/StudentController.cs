using System.IO;
using System.Xml;
using System.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.IdentityModel.Tokens;
using WebApi.Common;
using WebApi.dto.auth;
using WebApi.dto.Student;
using WebApi.Models;
using Microsoft.Identity.Client;
using WebApi.dto.Milestone;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StudentController : ControllerBase
{
    private readonly ApiDataContext dataContext;
    private readonly IPasswordHasher passwordHasher;
    private readonly IUploadPath uploadPath;

    public StudentController(ApiDataContext dataContext, IPasswordHasher passwordHasher, IUploadPath uploadPath)
    {
        this.dataContext = dataContext;
        this.passwordHasher = passwordHasher;
        this.uploadPath = uploadPath;
    }

    [Authorize(Roles = "Admin,Teacher"), HttpGet]
    public async Task<IActionResult> GetStudents()
    {
        var student = await dataContext.Students.Where(x => x.DeletedAt == null).ToListAsync();
           
        return Ok(new
        {
            message = "Succes to get data student",
            data = student
        });
    }

    [Authorize(Roles = "Student"), HttpGet("me")]
    public async Task<IActionResult> GetMyProfile()
    {
        ClaimsPrincipal claims = HttpContext.User;
        var user_id = claims.FindFirstValue(ClaimTypes.NameIdentifier);
        var roles = claims.FindFirstValue(ClaimTypes.Role);
        var profile = await dataContext.Students.Include(x => x.Attendances).FirstOrDefaultAsync(x => x.Id == Guid.Parse(user_id) && x.DeletedAt == null);
        if (profile == null)
            return Unauthorized();

        return Ok(new
        {
            message = "Get profile success",
            data = profile
        });
    }

    [Authorize(Roles = "Student"), HttpPost("me")]
    public async Task<IActionResult> UpdateMyProfile([FromForm] UpdateProfileRequest request)
    {
        ClaimsPrincipal claims = HttpContext.User;
        var user_id = claims.FindFirstValue(ClaimTypes.NameIdentifier);
        var roles = claims.FindFirstValue(ClaimTypes.Role);
        var profile = await dataContext.Students.FirstOrDefaultAsync(x => x.Id == Guid.Parse(user_id) && x.DeletedAt == null);
        if (profile == null)
            return Unauthorized();

        if (!request.FullName.IsNullOrEmpty())
            profile.FullName = request.FullName;
        if (!request.Username.IsNullOrEmpty())
            profile.Username = request.Username;
        if (request.BannerPicture != null)
        {
            if (!Directory.Exists(uploadPath.StudentPhoto()))
                Directory.CreateDirectory(uploadPath.StudentPhoto());
            var fileName = Guid.NewGuid().ToString() + request.BannerPicture.FileName;
            request.BannerPicture.CopyTo(new FileStream(Path.Combine(uploadPath.StudentPhoto(), fileName), FileMode.Create));
            if (!profile.BannerPicture.IsNullOrEmpty())
                if (System.IO.File.Exists(Path.Combine(uploadPath.StudentPhoto(), profile.BannerPicture)))
                    System.IO.File.Delete(Path.Combine(uploadPath.StudentPhoto(), profile.BannerPicture));
            profile.BannerPicture = Path.Combine("images/student", fileName);
        }
        if (request.ProfilePicture != null)
        {
            if (!Directory.Exists(uploadPath.StudentPhoto()))
                Directory.CreateDirectory(uploadPath.StudentPhoto());
            var fileName = Guid.NewGuid().ToString() + request.ProfilePicture.FileName;
            request.ProfilePicture.CopyTo(new FileStream(Path.Combine(uploadPath.StudentPhoto(), fileName), FileMode.Create));
            if (!profile.ProfilePicture.IsNullOrEmpty())
                if (System.IO.File.Exists(Path.Combine(uploadPath.StudentPhoto(), profile.ProfilePicture)))
                    System.IO.File.Delete(Path.Combine(uploadPath.StudentPhoto(), profile.ProfilePicture));
            profile.ProfilePicture = Path.Combine("images/student", fileName);
        }

        profile.UpdatedAt = DateTime.Now;
        dataContext.Students.Update(profile);
        await dataContext.SaveChangesAsync();
        return Ok(new
        {
            message = "Get profile success",
            data = profile
        });
    }

    [HttpPost("add")]
    public async Task<IActionResult> AddStudent([FromForm] RegisterStudent request)
    {
        var student = await dataContext.Students.FirstOrDefaultAsync(x => x.Username == request.Username);
        if (student != null)
            return Forbid();

        var data = new Student
        {
            FullName = request.FullName,
            Username = request.Username,
            Password = passwordHasher.HashPassword(request.Password),
            CreatedAt = DateTime.Now
        };

        dataContext.Students.Add(data);
        await dataContext.SaveChangesAsync();
        return Ok(new
        {
            message = "Student has been added",
            data = data
        });
    }
    [Authorize, HttpGet("{id:guid}")]
    public async Task<IActionResult> GetStudent(Guid id)
    {
        var student = await dataContext.Students.Include(x => x.Milestones).Include(x => x.Attendances).FirstOrDefaultAsync(x => x.Id == id && x.DeletedAt == null);
        if (student == null)
            return NotFound();
        student.AttendanceCuont = student.Attendances.Count;
        return Ok(new
        {
            message = "Get student success",
            data = student
        });
    }

    /// <summary>
    /// Update a test passed count for student
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns> <summary>
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    /// This endpoint is for teacher only
    [Authorize(Roles = "Teacher, Admin"), HttpPut("test/{id:guid}")]
    public async Task<IActionResult> UpdateStudentTess(UpdateStudentTestPassedRequest request, Guid id)
    {
        var student = await dataContext.Students.FirstOrDefaultAsync(x => x.Id == id && x.DeletedAt == null);
        if (student == null)
            return NotFound();

        student.TestPassed += request.TestPassedCount;
        student.UpdatedAt = DateTime.Now;
        dataContext.Students.Update(student);
        await dataContext.SaveChangesAsync();
        return Ok(student);
    }

    /// <summary>
    /// This nedpoint is for add or attahc milestone to a student
    /// this endpoint protected for teacher and admin only
    /// </summary>
    /// <returns></returns>
    [Authorize(Roles = "Teacher,Admin"), HttpPost("add/milestone")]
    public async Task<IActionResult> AddMilestoneToStudent(AddMilestoneToStudent request)
    {
        var milestone = await dataContext.Students.FirstOrDefaultAsync(x => x.Id == request.StudentId);

        foreach (var item in request.Milestones)
        {
            var student = await dataContext.Milestones.FirstOrDefaultAsync(x => x.Id == item);
            milestone.Milestones.Add(student);
        }
        dataContext.Students.Update(milestone);
        await dataContext.SaveChangesAsync();
        return Ok(milestone);
    }
}

