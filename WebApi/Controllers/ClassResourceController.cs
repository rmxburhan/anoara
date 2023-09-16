using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
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
        var classResources = await dataContext.ClassResources.Where(x => x.DeletedAt == null).Include(x => x.Class).ThenInclude(x => x.Teacher).ToListAsync();
        return Ok(new
        {
            message = "Get class resources success",
            data = classResources
        });
    }

    [Authorize(Roles = "Teacher,Admin"), HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateClassResource([FromForm] UpdateClassResoureceRequest request,Guid id)
    {
        var classResource = await dataContext.ClassResources.FirstOrDefaultAsync(x => x.Id == id && x.DeletedAt == null);
        if (classResource == null)
            return NotFound();

        if (!request.Title.IsNullOrEmpty())
            classResource.Title = request.Title;
        if (!request.Message.IsNullOrEmpty())
            classResource.Message = request.Message;
        if (request.AttachedFile != null)
        {
            var filename = Guid.NewGuid().ToString() + Path.GetExtension(request.AttachedFile.FileName);
            if (!Directory.Exists(uploadPath.ClassResource()))
                Directory.CreateDirectory(uploadPath.ClassResource());
            request.AttachedFile.CopyTo(new FileStream(Path.Combine(uploadPath.ClassResource(), filename), FileMode.Create));
            if (System.IO.File.Exists(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", classResource.FileLocation)))
                System.IO.File.Delete(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", classResource.FileLocation));
            classResource.FileLocation = Path.Combine(configuration.GetSection("UploadPath:ClassResources").Value, filename);
        }

        classResource.UpdatedAt = DateTime.Now;
        dataContext.ClassResources.Update(classResource);
        await dataContext.SaveChangesAsync();

        return Ok(new
        {
            message = "Class resource has been updated",
            data = classResource
        });
    }


    [Authorize(Roles = "Teacher,Admin"), HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteClassResource(Guid id)
    {
        var classResource = await dataContext.ClassResources.FirstOrDefaultAsync(x => x.Id == id && x.DeletedAt == null);
        if (classResource == null)
            return NotFound();

        classResource.DeletedAt = DateTime.Now;
        dataContext.ClassResources.Update(classResource);
        await dataContext.SaveChangesAsync();

        return NoContent();
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