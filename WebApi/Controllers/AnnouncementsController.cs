using System.Security.Claims;
using System.Data;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApi.dto.announcement;
using WebApi.Models;
using System.Data.Common;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Server.HttpSys;

namespace WebApi.Controllers;


[ApiController]
[Route("api/[controller]")]
public class AnnouncementsController : ControllerBase
{
    private readonly ApiDataContext dataContext;

    public AnnouncementsController(ApiDataContext dataContext)
    {
        this.dataContext = dataContext;
    }

    [Authorize, HttpGet]
    public async Task<IActionResult> GetAnnouncements()
    {
        var announcements = await dataContext.Announcements.Include(x => x.Teacher).Where(x => x.DeletedAt == null).ToListAsync();

        return Ok(new
        {
            messagae = "Halo",
            data = announcements
        }
        );
    }

    [Authorize, HttpGet("{id:guid}")]
    public async Task<IActionResult> GetAnnouncement(Guid id)
    {
        var announcement = await dataContext.Announcements.Include(x => x.AnnouncementComments).ThenInclude(x => x.Student).Include(x => x.Teacher).FirstOrDefaultAsync(x => x.Id == id && x.DeletedAt == null);
        if (announcement == null)
            return NotFound();

        return Ok(new
        {
            message = "Get data announcements succes",
            data = announcement
        });
    }

    [Authorize(Roles = "Teacher"), HttpPost]
    public async Task<IActionResult> AddAnnouncement(AddAnnouncementRequest request)
    {
        ClaimsPrincipal claims = HttpContext.User;
        var TeacherId = claims.FindFirstValue(ClaimTypes.NameIdentifier);
        var teacher = await dataContext.Teachers.FirstOrDefaultAsync(x => x.Id == Guid.Parse(TeacherId) && x.DeletedAt == null);

        if (teacher == null)
            return Unauthorized();

        var announcement = new Announcement
        {
            Title = request.Title,
            Description = request.Description,
            CreatedAt = DateTime.Now,
            TeacherId = teacher.Id
        };

        dataContext.Announcements.Add(announcement);
        await dataContext.SaveChangesAsync();

        return Ok(new
        {
            message = "Announcement has been added",
            data = announcement
        });
    }

    [Authorize(Roles = "Teacher"), HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateAnnouncement(UpdateAnnouncementRequest request, Guid id)
    {
        var announcement = await dataContext.Announcements.FirstOrDefaultAsync(x => x.Id == id && x.DeletedAt == null);
        if (announcement == null)
            return NotFound();

        if (!request.Title.IsNullOrEmpty())
            announcement.Title = request.Title;
        if (!request.Description.IsNullOrEmpty())
            announcement.Description = request.Description;

        announcement.UpdatedAt = DateTime.Now;
        dataContext.Announcements.Update(announcement);
        await dataContext.SaveChangesAsync();
        return Ok(new
        {
            messagae = "Announcement has been updated",
            data = announcement
        });
    }

    [Authorize(Roles = "Teacher"), HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteAnnouncem(Guid id)
    {

        var announcement = await dataContext.Announcements.FirstOrDefaultAsync(x => x.Id == id && x.DeletedAt == null);
        if (announcement == null)
            return NotFound();

        announcement.DeletedAt = DateTime.Now;
        dataContext.Announcements.Update(announcement);
        await dataContext.SaveChangesAsync();

        return NoContent();
    }

    [Authorize(Roles = "Student"), HttpPost("{id:guid}/comment")]
    public async Task<IActionResult> AddCommentAnnouncement(AddAnnouncementCommentRequest request, Guid id)
    {
        ClaimsPrincipal claimsToken = HttpContext.User;
        var student_id = claimsToken.FindFirstValue(ClaimTypes.NameIdentifier);
        var student = await dataContext.Students.FirstOrDefaultAsync(x => x.Id == Guid.Parse(student_id) && x.DeletedAt == null);
        if (student == null)
            return Unauthorized();
        var comment = new AnnouncementComment
        {
            CreatedAt = DateTime.Now,
            Message = request.Message,
            StudentId = student.Id,
            AnnouncementId = id
        };
        dataContext.AnnouncementComments.Add(comment);
        await dataContext.SaveChangesAsync();
        return Ok(new
        {
            messagae = "Comment has been created",
            data = comment
        });
    }
}