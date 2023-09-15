using System.Reflection.Metadata.Ecma335;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApi.Common;
using WebApi.dto.auth;
using WebApi.dto.Student;
using WebApi.Models;

namespace WebApi.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly ApiDataContext dataContext;
    private readonly IJwtTokenGenerator tokenGenerator;
    private readonly IPasswordHasher passwordHasher;

    public AuthController(ApiDataContext dataContext, IJwtTokenGenerator tokenGenerator, IPasswordHasher passwordHasher)
    {
        this.dataContext = dataContext;
        this.tokenGenerator = tokenGenerator;
        this.passwordHasher = passwordHasher;
    }

    [HttpPost("student/login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var student = await dataContext.Students.FirstOrDefaultAsync(x => x.Username == request.Username);
        if (student == null)
            return NotFound();

        if (student.Password != passwordHasher.HashPassword(request.Password))
            return BadRequest();

        var token = tokenGenerator.GenerateToken(student.Id.ToString(), "Student");
        var response = new AuthenticationResponse(
            token.Token,
            token.ExpiredAt
        );
        return Ok(response);
    }

    [HttpPost("teacher/login")]
    public async Task<IActionResult> TeacherLogin(LoginRequest request)
    {
        var Teacher = await dataContext.Teachers.FirstOrDefaultAsync(x => x.Username == request.Username);
        if (Teacher == null)
            return NotFound();

        if (Teacher.Password != passwordHasher.HashPassword(request.Password))
            return BadRequest();

        var token = tokenGenerator.GenerateToken(Teacher.Id.ToString(), "Teacher");
        var response = new AuthenticationResponse(
            token.Token,
            token.ExpiredAt
        );
        return Ok(response);
    }
}