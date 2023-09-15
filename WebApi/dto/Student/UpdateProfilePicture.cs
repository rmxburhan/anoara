namespace WebApi.dto.Student;

public record UpdateProfilePicture(
    IFormFile? ProfilePictuer,
    IFormFile? BannerPicture
);