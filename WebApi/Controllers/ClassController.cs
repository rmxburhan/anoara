using System.Security.Principal;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApi.dto.Class;
using WebApi.Models;

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
    public async Task<IActionResult> GetClass()
    {
        var classes = await dataContext.Classes.Where(x => x.DeletedAt == null).ToListAsync();
        return Ok(new
        {
            message = "Get data classes success",
            data = classes
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
}