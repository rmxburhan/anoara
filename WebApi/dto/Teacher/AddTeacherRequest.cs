namespace WebApi.dto.Teacher;

public class AddTeacherRequest
{
    public string Name { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public IFormFile? ProfilePicture { get; set; }
}