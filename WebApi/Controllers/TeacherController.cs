using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApi.Common;
using WebApi.dto.Teacher;
using WebApi.Models;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TeacherController : ControllerBase
{
    private readonly ApiDataContext dataContext;
    private readonly IPasswordHasher passwordHasher;
    private readonly IUploadPath uploadPath;
    private readonly IConfiguration configuration;

    public TeacherController(ApiDataContext dataContext, IPasswordHasher passwordHasher, IUploadPath uploadPath, IConfiguration configuration)
    {
        this.dataContext = dataContext;
        this.passwordHasher = passwordHasher;
        this.uploadPath = uploadPath;
        this.configuration = configuration;
    }

    [HttpGet]
    public async Task<IActionResult> GetTeachers()
    {
        var teachers = await dataContext.Teachers.Where(x => x.DeletedAt == null).ToListAsync();

        return Ok(new
        {
            message = "Get teacher success",
            data = teachers
        });
    }

    [HttpPost]
    public async Task<IActionResult> AddTeacher([FromForm] AddTeacherRequest request)
    {
        var teacher = await dataContext.Teachers.FirstOrDefaultAsync(x => x.Username == request.Username && x.DeletedAt == null);

        if (teacher != null)
            return Forbid();

        var data = new Teacher
        {
            Name = request.Name,
            Username = request.Username,
            Password = passwordHasher.HashPassword(request.Password),
            CreatedAt = DateTime.Now
        };

        if (request.ProfilePicture != null)
        {
            var filename = Guid.NewGuid().ToString() + request.ProfilePicture.FileName;
            if (!Directory.Exists(uploadPath.TeacherPhoto()))
                Directory.CreateDirectory(uploadPath.TeacherPhoto());

            request.ProfilePicture.CopyTo(new FileStream(Path.Combine(uploadPath.TeacherPhoto(), filename), FileMode.Create));
            data.ProfilePicture = Path.Combine("images", configuration.GetSection("UploadPath:Teacher").Value, filename);
        }

        dataContext.Teachers.Add(data);
        await dataContext.SaveChangesAsync();

        return Ok(new
        {
            message = "Teacher has been added",
            data = data
        });
    }
}