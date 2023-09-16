using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApi.dto.Attendance;
using WebApi.Models;

namespace WebApi.Controllers;

[ApiController]
[Route("api/attendances")]
public class AttendanceController : ControllerBase
{
    private readonly ApiDataContext dataContext;

    public AttendanceController(ApiDataContext dataContext)
    {
        this.dataContext = dataContext;
    }

    [Authorize, HttpPost("add")]
    public async Task<IActionResult> AddAttendances(AddAttendanceRequest request)
    {
        var Class = await dataContext.Classes.FirstOrDefaultAsync(x => x.DeletedAt == null && x.Id == request.ClassId);
        if (Class == null)
            return NotFound();


        foreach (var item in request.Students)
        {
            var attendanceExist = await dataContext.Attendances.FirstOrDefaultAsync(x => x.ClassId == Class.Id && x.StudentId == item);
            if (attendanceExist != null)
                continue;

            var data = new Attendance
            {
                Date = DateTime.Now,
                StudentId = item,
                ClassId = request.ClassId,
                CreatedAt = DateTime.Now
            };
            dataContext.Attendances.Add(data);
            Class.Attendances.Add(data);
        }
        dataContext.Classes.Update(Class);
        await dataContext.SaveChangesAsync();
        return Ok(new
        {
            message = "Add attendance success",
            data = Class
        });
    }
}