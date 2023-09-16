using System.Security.Principal;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApi.dto.Class;
using WebApi.Models;
using Org.BouncyCastle.Crypto.Prng;

namespace WebApi.Controllers;

[ApiController]
[Route("api/class")]
public class ClassController : ControllerBase
{
    private readonly ApiDataContext dataContext;

    public ClassController(ApiDataContext dataContext)
    {
        this.dataContext = dataContext;
    }

    [Authorize, HttpGet]
    public async Task<IActionResult> GetClasses()
    {
        var classes = await dataContext.Classes.Include(x => x.Teacher).Where(x => x.DeletedAt == null).ToListAsync();
        return Ok(new
        {
            message = "Get data classes success",
            data = classes
        });
    }

    [Authorize(Roles = "Teacher, Admin"), HttpGet("{id:guid}")]
    public async Task<IActionResult> GetClass(Guid id)
    {
        var Class = await dataContext.Classes.Include(x => x.Teacher).Include(x => x.Attendances).ThenInclude(x => x.Student).FirstOrDefaultAsync(x => x.DeletedAt == null && x.Id == id);
        if (Class == null)
            return NotFound();
        return Ok(new
        {
            message = "Get data class success",
            data = Class
        });
    }

    [Authorize(Roles = "Admin, Teacher"), HttpPost]
    public async Task<IActionResult> AddDataClass(AddClassRequest request)
    {
        ClaimsPrincipal tokenClaims = HttpContext.User;
        var role = tokenClaims.FindFirstValue(ClaimTypes.Role);
        if (role == "Admin")
        {
            if (!request.TecherId.HasValue)
                return BadRequest();
            var teacher = await dataContext.Teachers.FirstOrDefaultAsync(x => x.DeletedAt == null && x.Id == request.TecherId);
            if (teacher == null)
                return BadRequest();
            var kelas = new Class
            {
                Topic = request.Topic,
                CreatedAt = DateTime.Now,
                Time = request.Time,
                TeacherId = teacher.Id
            };
            dataContext.Classes.Add(kelas);
            await dataContext.SaveChangesAsync();
            return Ok(new
            {
                message = "Class has been added",
                data = kelas
            });
        }
        else if (role == "Teacher")
        {
            var teacher_id = tokenClaims.FindFirstValue(ClaimTypes.NameIdentifier);
            var teacher = await dataContext.Teachers.FirstOrDefaultAsync(x => x.DeletedAt == null && x.Id == Guid.Parse(teacher_id));
            if (teacher == null)
                return Unauthorized();

            var kelas = new Class
            {
                Topic = request.Topic,
                CreatedAt = DateTime.Now,
                Time = request.Time,
                TeacherId = teacher.Id
            };

            dataContext.Classes.Add(kelas);
            await dataContext.SaveChangesAsync();
            return Ok(new
            {
                message = "Class has been added",
                data = kelas
            });
        }
        else
        {
            return Forbid();
        }
    }

    [Authorize(Roles = "Admin, Teacher"), HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateDataClass(AddClassRequest request, Guid id)
    {
        ClaimsPrincipal tokenClaims = HttpContext.User;
        var role = tokenClaims.FindFirstValue(ClaimTypes.Role);
        if (role == "Admin")
        {
            if (!request.TecherId.HasValue)
                return BadRequest();
            var teacher = await dataContext.Teachers.FirstOrDefaultAsync(x => x.DeletedAt == null && x.Id == request.TecherId);
            if (teacher == null)
                return BadRequest();
            var kelas = await dataContext.Classes.FirstOrDefaultAsync(x => x.Id == id && x.DeletedAt == null);
            if (kelas == null)
                return NotFound();

            if (!string.IsNullOrEmpty(request.Topic))
                kelas.Topic = request.Topic;
            if (request.Time != null)
                kelas.Time = request.Time;
            if (request.TecherId.HasValue)
                kelas.TeacherId = teacher.Id;

            kelas.UpdatedAt = DateTime.Now;
            dataContext.Classes.Update(kelas);
            await dataContext.SaveChangesAsync();
            return Ok(new
            {
                message = "Class has been updated",
                data = kelas
            });
        }
        else if (role == "Teacher")
        {
            var teacher_id = tokenClaims.FindFirstValue(ClaimTypes.NameIdentifier);
            var teacher = await dataContext.Teachers.FirstOrDefaultAsync(x => x.DeletedAt == null && x.Id == Guid.Parse(teacher_id));
            if (teacher == null)
                return Unauthorized();

            var kelas = await dataContext.Classes.FirstOrDefaultAsync(x => x.Id == id && x.DeletedAt == null);
            if (kelas == null)
                return NotFound();

            if (!string.IsNullOrEmpty(request.Topic))
                kelas.Topic = request.Topic;
            if (request.Time != null)
                kelas.Time = request.Time;

            kelas.UpdatedAt = DateTime.Now;
            dataContext.Classes.Update(kelas);
            await dataContext.SaveChangesAsync();
            return Ok(new
            {
                message = "Class has been updated",
                data = kelas
            });
        }
        else
        {
            return Forbid();
        }
    }

    [Authorize, HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteClass(Guid id)
    {
        var Class = await dataContext.Classes.FirstOrDefaultAsync(x => x.Id == id && x.DeletedAt == null);
        if (Class == null)
            return NotFound();

        Class.DeletedAt = DateTime.Now;
        dataContext.Classes.Update(Class);
        await dataContext.SaveChangesAsync();
        return NoContent();
    }
}