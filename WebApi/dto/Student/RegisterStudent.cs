namespace WebApi.dto.Student;

public class RegisterStudent
{
    public string FullName { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public IFormFile? ProfilePicture { get; set; }
    public IFormFile? BannerPicture { get; set; }
}