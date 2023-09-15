using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

    [Authorize(Roles = "Teacher"), HttpPost]
    public async Task<IActionResult> AddAttendances(AddAttendanceRequest request)
    {
        List<Attendance> datas = new List<Attendance>();

        foreach (var item in request.Students)
        {
            datas.Add(new Attendance
            {
                Date = DateTime.Now,
                StudentId = item,
                ClassId = request.ClassId,
                CreatedAt = DateTime.Now
            });
        }

        await dataContext.Attendances.AddRangeAsync(datas);
        return Ok(new
        {
            message = "Add attendance success",
            data = datas
        });
    }
}