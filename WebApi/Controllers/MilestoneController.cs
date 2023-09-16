using System.Net;
using System.Security.AccessControl;
using System.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using WebApi.Common;
using WebApi.dto.Milestone;
using WebApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore.Storage;

namespace WebApi.Controllers;

[ApiController]
[Route("api/milestone")]
public class MilestoneController : ControllerBase
{
    private readonly ApiDataContext dataContext;
    private readonly IUploadPath uploadPath;

    public MilestoneController(ApiDataContext dataContext, IUploadPath uploadPath)
    {
        this.dataContext = dataContext;
        this.uploadPath = uploadPath;
    }

    [Authorize, HttpGet]
    public async Task<IActionResult> GetMilestones([FromQuery] FilterMilestone filters)
    {
        var milestones = dataContext.Milestones.Where(x => x.DeletedAt == null);
       
        var result = await milestones.ToListAsync();
        return Ok(new
        {
            message = "Get milestones succes",
            data = milestones
        });
    }

    [Authorize(Roles = "Admin,Teacher"), HttpGet("{id:guid}")]
    public async Task<IActionResult> getMilestone(Guid id)
    {
        var milestone = await dataContext.Milestones.FirstOrDefaultAsync(x => x.Id == id && x.DeletedAt == null);
        if (milestone == null)
            return NotFound();

        return Ok(new
        {
            message = "Get milestones succes",
            data = milestone
        });
    }

    [Authorize(Roles = "Admin,Teacher"), HttpPost]
    public async Task<IActionResult> AddMilestone([FromForm] AddMilestoneRequest request)
    {
        var milestone = new Milestone
        {
            Title = request.Title,
            Description = request.Description,
            CreatedAt = DateTime.Now
        };
        if (request.Image != null)
        {
            string fileName = Guid.NewGuid().ToString() + request.Image.FileName;
            if (!Directory.Exists(uploadPath.MilestonePath()))
                Directory.CreateDirectory(uploadPath.MilestonePath());
            request.Image.CopyTo(new FileStream(Path.Combine(uploadPath.MilestonePath(), fileName), FileMode.Create));
            milestone.Image = fileName;
        }
        dataContext.Milestones.Add(milestone);
        await dataContext.SaveChangesAsync();
        return Ok(new
        {
            message = "Milestone has been added",
            data = milestone
        });
    }

    [Authorize(Roles = "Admin,Teacher"), HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteMilestone(Guid id)
    {
        var milestone = await dataContext.Milestones.FirstOrDefaultAsync(x => x.Id == id && x.DeletedAt == null);

        if (milestone == null)
            return NotFound();

        milestone.DeletedAt = DateTime.Now;
        dataContext.Milestones.Update(milestone);
        await dataContext.SaveChangesAsync();
        return NoContent();
    }

    [Authorize(Roles = "Admin,Teacher"), HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateMilestone([FromForm] UpdateMilestoneRequest request, Guid id)
    {
        var milestone = await dataContext.Milestones.FirstOrDefaultAsync(x => x.Id == id && x.DeletedAt == null);

        if (milestone == null)
            return NotFound();

        if (!request.Title.IsNullOrEmpty())
            milestone.Title = request.Title;
        if (!request.Description.IsNullOrEmpty())
            milestone.Description = request.Description;
        if (request.Image != null)
        {
            string fileName = Guid.NewGuid().ToString() + request.Image.FileName;
            if (!Directory.Exists(uploadPath.MilestonePath()))
                Directory.CreateDirectory(uploadPath.MilestonePath());
            request.Image.CopyTo(new FileStream(Path.Combine(uploadPath.MilestonePath(), fileName), FileMode.Create));
            milestone.Image = fileName;
        }
        milestone.UpdatedAt = DateTime.Now;
        dataContext.Milestones.Update(milestone);

        await dataContext.SaveChangesAsync();
        return Ok(new
        {
            message = "Milestone has been updated",
            data = milestone
        });
    }

 

}