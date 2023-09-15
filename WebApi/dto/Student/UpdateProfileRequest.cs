namespace WebApi.dto.Student;

public class UpdateProfileRequest
{

    public string? FullName { get; set; }
    public string? Username { get; set; }
    public IFormFile? BannerPicture { get; set; }
    public IFormFile? ProfilePicture { get; set; }
}