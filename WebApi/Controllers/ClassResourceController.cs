using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApi.Common;
using WebApi.dto.ClassResources;
using WebApi.Models;

namespace WebApi.Controllers;


[ApiController]
[Route("api/classresource")]
public class ClassResourceController : ControllerBase
{
    private readonly ApiDataContext dataContext;
    private readonly IConfiguration configuration;
    private readonly IUploadPath uploadPath;

    public ClassResourceController(ApiDataContext dataContext, IConfiguration configuration, IUploadPath uploadPath)
    {
        this.dataContext = dataContext;
        this.configuration = configuration;
        this.uploadPath = uploadPath;
    }

    [Authorize, HttpGet]
    public async Task<IActionResult> GetClassResources()
    {
        var classResources = await dataContext.ClassResources.Include(x => x.Class).ThenInclude(x => x.Teacher).ToListAsync();
        return Ok(new
        {
            message = "Get class resources success",
            data = classResources
        });
    }

    [Authorize(Roles = "Teacher,Admin"), HttpPost]
    public async Task<IActionResult> AddClassResources([FromForm] AddClassResoureceRequest request)
    {
        var classresource = new ClassResources
        {
            Title = request.Title,
            Message = request.Message,
            ClassId = request.ClassId,
            CreatedAt = DateTime.Now
        };

        if (request.AttachedFile != null)
        {
            var filename = Guid.NewGuid().ToString() + Path.GetExtension(request.AttachedFile.FileName);
            if (!Directory.Exists(uploadPath.ClassResource()))
                Directory.CreateDirectory(uploadPath.ClassResource());
            request.AttachedFile.CopyTo(new FileStream(Path.Combine(uploadPath.ClassResource(), filename), FileMode.Create));
            classresource.FileLocation = Path.Combine(configuration.GetSection("UploadPath:ClassResources").Value, filename);
        }
        else
        {
            return BadRequest();
        }

        dataContext.ClassResources.Add(classresource);
        await dataContext.SaveChangesAsync();
        return Ok(classresource);
    }
}